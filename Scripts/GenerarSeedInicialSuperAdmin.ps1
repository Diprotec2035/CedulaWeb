<#
Genera un seed SQL para el primer superadministrador de Cédula.

No se conecta a MySQL ni ejecuta sentencias. Solicita los datos localmente,
genera PBKDF2-SHA256 con la misma configuración que PasswordHelperNegocio y
escribe el SQL transaccional en la consola.
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-PositiveEnvironmentInt {
    param(
        [string]$Name,
        [int]$DefaultValue
    )

    $rawValue = [Environment]::GetEnvironmentVariable($Name, [EnvironmentVariableTarget]::Machine)
    $parsedValue = 0

    if ([int]::TryParse($rawValue, [ref]$parsedValue) -and $parsedValue -gt 0) {
        return $parsedValue
    }

    return $DefaultValue
}

function ConvertTo-Hex {
    param([byte[]]$Bytes)

    return [BitConverter]::ToString($Bytes).Replace('-', '')
}

function ConvertTo-SqlLiteral {
    param([string]$Value)

    return "'" + $Value.Replace("'", "''") + "'"
}

function Read-PlainTextPassword {
    $securePassword = Read-Host 'Contraseña inicial' -AsSecureString
    $pointer = [IntPtr]::Zero

    try {
        $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
        return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
    }
    finally {
        if ($pointer -ne [IntPtr]::Zero) {
            [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer)
        }
    }
}

$rut = (Read-Host 'RUT del superadministrador').Trim().ToUpperInvariant()
$nombre = (Read-Host 'Nombre').Trim().ToUpperInvariant()
$apellido = (Read-Host 'Apellido').Trim().ToUpperInvariant()
$email = (Read-Host 'Email').Trim().ToLowerInvariant()
$telefono = (Read-Host 'Teléfono').Trim()
$password = Read-PlainTextPassword
$passwordConfirmation = Read-PlainTextPassword

if ([string]::IsNullOrWhiteSpace($rut) -or
    [string]::IsNullOrWhiteSpace($nombre) -or
    [string]::IsNullOrWhiteSpace($apellido) -or
    [string]::IsNullOrWhiteSpace($email) -or
    [string]::IsNullOrWhiteSpace($telefono) -or
    [string]::IsNullOrWhiteSpace($password)) {
    throw 'Todos los datos son obligatorios.'
}

if ($password -cne $passwordConfirmation) {
    throw 'Las contraseñas no coinciden.'
}

$saltSizeBytes = Get-PositiveEnvironmentInt 'EV-Cedula-Password-Salt' 16
$hashSizeBytes = Get-PositiveEnvironmentInt 'EV-Cedula-Password-Hash' 32
$iterations = Get-PositiveEnvironmentInt 'EV-Cedula-Password-Iterations' 100000
$algorithmId = [Environment]::GetEnvironmentVariable('EV-Cedula-Password-AlgoritmoId', [EnvironmentVariableTarget]::Machine)

if ([string]::IsNullOrWhiteSpace($algorithmId)) {
    $algorithmId = "PBKDF2-SHA256-$iterations"
}

$salt = New-Object byte[] $saltSizeBytes
([Security.Cryptography.RandomNumberGenerator]::Create()).GetBytes($salt)

try {
    $pbkdf2 = New-Object Security.Cryptography.Rfc2898DeriveBytes(
        $password,
        $salt,
        $iterations,
        [Security.Cryptography.HashAlgorithmName]::SHA256
    )

    try {
        $hash = $pbkdf2.GetBytes($hashSizeBytes)
    }
    finally {
        $pbkdf2.Dispose()
    }
}
finally {
    $password = $null
    $passwordConfirmation = $null
}

$hashHex = ConvertTo-Hex $hash
$saltHex = ConvertTo-Hex $salt

$sql = @"
-- Ejecutar una sola vez en dpt_cedula_2.
-- Este seed crea el primer superadministrador sin simular una bitácora previa.
START TRANSACTION;

INSERT INTO tbl_usuario
(
    USU_RUT,
    USU_NOMBRE,
    USU_APELLIDO,
    USU_UPDATED_AT
)
VALUES
(
    $(ConvertTo-SqlLiteral $rut),
    $(ConvertTo-SqlLiteral $nombre),
    $(ConvertTo-SqlLiteral $apellido),
    CURRENT_TIMESTAMP(6)
)
ON DUPLICATE KEY UPDATE
    USU_NOMBRE = VALUES(USU_NOMBRE),
    USU_APELLIDO = VALUES(USU_APELLIDO),
    USU_UPDATED_AT = CURRENT_TIMESTAMP(6);

INSERT INTO tbl_super_admin
(
    SA_RUT,
    SA_PASSWORD_HASH,
    SA_PASSWORD_SALT,
    SA_PASSWORD_ALGORITMO,
    SA_EMAIL,
    SA_TELEFONO,
    SA_ESTADO,
    SA_MFA_REQUERIDO,
    SA_UPDATED_AT
)
VALUES
(
    $(ConvertTo-SqlLiteral $rut),
    UNHEX('$hashHex'),
    UNHEX('$saltHex'),
    $(ConvertTo-SqlLiteral $algorithmId),
    $(ConvertTo-SqlLiteral $email),
    $(ConvertTo-SqlLiteral $telefono),
    0,
    0,
    CURRENT_TIMESTAMP(6)
);

COMMIT;
"@

Write-Host ''
Write-Host 'El SQL contiene un hash de credencial. No lo guardes en archivos compartidos ni lo reutilices.' -ForegroundColor Yellow
Write-Host 'Verifica que el RUT no exista en tbl_super_admin antes de ejecutarlo.' -ForegroundColor Yellow
Write-Output $sql
