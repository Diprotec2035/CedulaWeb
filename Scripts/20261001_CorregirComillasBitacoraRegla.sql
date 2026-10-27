/*
    dpt_cedula_2: corrige las comillas del detalle de la bitacora de reglas.

    SP_INS_REGLA y SP_UPD_REGLA armaban el detalle con '""""' (cuatro comillas)
    y SP_UPD_REGLA_ITEM con '""' (dos comillas); MySQL las guarda literalmente.
    Se reemplazan por '"' (una comilla), igual que los procedimientos de
    Inventario. No se modifica ninguna otra linea de los procedimientos.

    Ejecutar POR BLOQUES, con respaldo y con la cuenta que puede recrear
    procedimientos con DEFINER root@% (el mismo de los scripts del 28-09-2026).
*/

USE dpt_cedula_2;

/* BLOQUE 1 - Diagnostico: procedimientos y filas de bitacora afectadas. */
SELECT ROUTINE_NAME,
       ROUTINE_DEFINITION LIKE '%""%' AS tiene_comillas_dobles
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = 'dpt_cedula_2'
  AND ROUTINE_NAME IN ('SP_INS_REGLA', 'SP_UPD_REGLA', 'SP_UPD_REGLA_ITEM');

SELECT COUNT(*) AS filas_afectadas
FROM tbl_regla_bitacora
WHERE RB_DETALLE LIKE '%""%';

/* BLOQUE 2 - Recrear los tres procedimientos con una sola comilla. */
DELIMITER $$

DROP PROCEDURE IF EXISTS SP_INS_REGLA$$
CREATE DEFINER=`root`@`%` PROCEDURE SP_INS_REGLA(
    IN P_EMP_RUT VARCHAR(10),
    IN P_REG_NOMBRE VARCHAR(100),
    IN P_REG_NOMBRE_APELLIDO TINYINT UNSIGNED,
    IN P_REG_EMPRESA TINYINT UNSIGNED,
    IN P_REG_PATENTE TINYINT UNSIGNED,
    IN P_REG_COMENTARIO TINYINT UNSIGNED,
    IN P_REG_FOTOGRAFIA TINYINT UNSIGNED,
    IN P_REG_ENTRADA_SALIDA TINYINT UNSIGNED,
    IN P_REG_LISTA_BLANCA_NEGRA TINYINT UNSIGNED,
    IN P_REG_ELIMINA_ENVIADOS TINYINT UNSIGNED,
    IN P_REG_ESTADO TINYINT UNSIGNED,
    IN P_USU_RUT VARCHAR(10),
    OUT P_RETORNO TINYINT UNSIGNED
)
BEGIN
    DECLARE V_REG_ID INT UNSIGNED DEFAULT 0;
    DECLARE V_EMP_RUT VARCHAR(10) DEFAULT '';
    DECLARE V_REG_NOMBRE VARCHAR(100) DEFAULT '';
    DECLARE V_USU_RUT VARCHAR(10) DEFAULT '';
    DECLARE V_DETALLE TEXT DEFAULT '';

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET P_RETORNO = 255;
    END;

    SET V_EMP_RUT = UPPER(TRIM(P_EMP_RUT));
    SET V_REG_NOMBRE = UPPER(TRIM(P_REG_NOMBRE));
    SET V_USU_RUT = UPPER(TRIM(P_USU_RUT));

    START TRANSACTION;

    INSERT INTO tbl_regla
    (
        EMP_RUT,
        REG_NOMBRE,
        REG_NOMBRE_APELLIDO,
        REG_EMPRESA,
        REG_PATENTE,
        REG_COMENTARIO,
        REG_FOTOGRAFIA,
        REG_ENTRADA_SALIDA,
        REG_LISTA_BLANCA_NEGRA,
        REG_ELIMINA_ENVIADOS,
        REG_ESTADO,
        REG_UPDATED_AT
    )
    VALUES
    (
        V_EMP_RUT,
        V_REG_NOMBRE,
        P_REG_NOMBRE_APELLIDO,
        P_REG_EMPRESA,
        P_REG_PATENTE,
        P_REG_COMENTARIO,
        P_REG_FOTOGRAFIA,
        P_REG_ENTRADA_SALIDA,
        P_REG_LISTA_BLANCA_NEGRA,
        P_REG_ELIMINA_ENVIADOS,
        P_REG_ESTADO,
        CURRENT_TIMESTAMP(6)
    );

    SET V_REG_ID = LAST_INSERT_ID();

    SET V_DETALLE = CONCAT(
        'Nombre: "', V_REG_NOMBRE, '"; ',
        'Pide nombre: "', IF(P_REG_NOMBRE_APELLIDO = 1, 'Sí', 'No'), '"; ',
        'Pide empresa: "', IF(P_REG_EMPRESA = 1, 'Sí', 'No'), '"; ',
        'Pide patente: "', IF(P_REG_PATENTE = 1, 'Sí', 'No'), '"; ',
        'Pide comentario: "', IF(P_REG_COMENTARIO = 1, 'Sí', 'No'), '"; ',
        'Pide fotografía: "', IF(P_REG_FOTOGRAFIA = 1, 'Sí', 'No'), '"; ',
        'Indica entrada/salida: "', IF(P_REG_ENTRADA_SALIDA = 1, 'Sí', 'No'), '"; ',
        'Lista: "', CASE P_REG_LISTA_BLANCA_NEGRA
            WHEN 1 THEN 'Lista blanca'
            WHEN 2 THEN 'Lista negra'
            ELSE 'Sin lista'
        END, '"; ',
        'Permite eliminar enviados: "', IF(P_REG_ELIMINA_ENVIADOS = 1, 'Sí', 'No'), '"; ',
        'Estado: "', IF(P_REG_ESTADO = 0, 'Vigente', 'Anulado'), '"'
    );

    CALL SP_INS_REGLA_BITACORA(V_REG_ID, V_USU_RUT, 'CREAR', 'Regla creada', V_DETALLE);

    COMMIT;
    SET P_RETORNO = 0;
END$$

DROP PROCEDURE IF EXISTS SP_UPD_REGLA$$
CREATE DEFINER=`root`@`%` PROCEDURE SP_UPD_REGLA(
    IN P_REG_ID INT UNSIGNED,
    IN P_REG_NOMBRE VARCHAR(100),
    IN P_REG_NOMBRE_APELLIDO TINYINT UNSIGNED,
    IN P_REG_EMPRESA TINYINT UNSIGNED,
    IN P_REG_PATENTE TINYINT UNSIGNED,
    IN P_REG_COMENTARIO TINYINT UNSIGNED,
    IN P_REG_FOTOGRAFIA TINYINT UNSIGNED,
    IN P_REG_ENTRADA_SALIDA TINYINT UNSIGNED,
    IN P_REG_LISTA_BLANCA_NEGRA TINYINT UNSIGNED,
    IN P_REG_ELIMINA_ENVIADOS TINYINT UNSIGNED,
    IN P_REG_ESTADO TINYINT UNSIGNED,
    IN P_REG_UPDATED_AT_ORIGINAL DATETIME(6),
    IN P_USU_RUT VARCHAR(10),
    OUT P_RETORNO TINYINT UNSIGNED
)
BEGIN
    DECLARE V_EXISTE INT DEFAULT 0;
    DECLARE V_PERFIL_VIGENTE INT DEFAULT 0;
    DECLARE V_REG_NOMBRE_ANTERIOR VARCHAR(100) DEFAULT '';
    DECLARE V_ELIMINA_ANTERIOR TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_ESTADO_ANTERIOR TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_UPDATED_AT_ACTUAL DATETIME(6) DEFAULT NULL;
    DECLARE V_NOMBRE_ANTERIOR TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_EMPRESA_ANTERIOR TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_PATENTE_ANTERIOR TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_COMENTARIO_ANTERIOR TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_FOTOGRAFIA_ANTERIOR TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_ENTRADA_SALIDA_ANTERIOR TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_LISTA_ANTERIOR TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_REG_NOMBRE VARCHAR(100) DEFAULT '';
    DECLARE V_USU_RUT VARCHAR(10) DEFAULT '';
    DECLARE V_DETALLE TEXT DEFAULT '';

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET P_RETORNO = 255;
    END;

    SET V_REG_NOMBRE = UPPER(TRIM(P_REG_NOMBRE));
    SET V_USU_RUT = UPPER(TRIM(P_USU_RUT));

    START TRANSACTION;

    SELECT COUNT(1) INTO V_EXISTE
    FROM tbl_regla
    WHERE REG_ID = P_REG_ID;

    IF V_EXISTE = 0 THEN
        ROLLBACK;
        SET P_RETORNO = 1;
    ELSE
        SELECT
            R.REG_NOMBRE,
            R.REG_ELIMINA_ENVIADOS,
            R.REG_ESTADO,
            R.REG_UPDATED_AT,
            R.REG_NOMBRE_APELLIDO,
            R.REG_EMPRESA,
            R.REG_PATENTE,
            R.REG_COMENTARIO,
            R.REG_FOTOGRAFIA,
            R.REG_ENTRADA_SALIDA,
            R.REG_LISTA_BLANCA_NEGRA
        INTO
            V_REG_NOMBRE_ANTERIOR,
            V_ELIMINA_ANTERIOR,
            V_ESTADO_ANTERIOR,
            V_UPDATED_AT_ACTUAL,
            V_NOMBRE_ANTERIOR,
            V_EMPRESA_ANTERIOR,
            V_PATENTE_ANTERIOR,
            V_COMENTARIO_ANTERIOR,
            V_FOTOGRAFIA_ANTERIOR,
            V_ENTRADA_SALIDA_ANTERIOR,
            V_LISTA_ANTERIOR
        FROM tbl_regla R
        WHERE R.REG_ID = P_REG_ID
        FOR UPDATE;

        IF NOT (V_UPDATED_AT_ACTUAL <=> P_REG_UPDATED_AT_ORIGINAL) THEN
            ROLLBACK;
            SET P_RETORNO = 3;
        ELSE
            SELECT COUNT(1) INTO V_PERFIL_VIGENTE
            FROM tbl_perfil
            WHERE REG_ID = P_REG_ID
              AND PER_ESTADO = 0;

            IF P_REG_ESTADO = 1
                AND V_ESTADO_ANTERIOR <> 1
                AND V_PERFIL_VIGENTE > 0 THEN
                ROLLBACK;
                SET P_RETORNO = 2;
            ELSE
                UPDATE tbl_regla
                SET REG_NOMBRE = V_REG_NOMBRE,
                    REG_NOMBRE_APELLIDO = P_REG_NOMBRE_APELLIDO,
                    REG_EMPRESA = P_REG_EMPRESA,
                    REG_PATENTE = P_REG_PATENTE,
                    REG_COMENTARIO = P_REG_COMENTARIO,
                    REG_FOTOGRAFIA = P_REG_FOTOGRAFIA,
                    REG_ENTRADA_SALIDA = P_REG_ENTRADA_SALIDA,
                    REG_LISTA_BLANCA_NEGRA = P_REG_LISTA_BLANCA_NEGRA,
                    REG_ELIMINA_ENVIADOS = P_REG_ELIMINA_ENVIADOS,
                    REG_ESTADO = P_REG_ESTADO,
                    REG_UPDATED_AT = CURRENT_TIMESTAMP(6)
                WHERE REG_ID = P_REG_ID;

                IF V_REG_NOMBRE_ANTERIOR <> V_REG_NOMBRE THEN
                    SET V_DETALLE = CONCAT(V_DETALLE, 'Nombre: "', V_REG_NOMBRE_ANTERIOR, '" -> "', V_REG_NOMBRE, '"; ');
                END IF;

                IF V_NOMBRE_ANTERIOR <> P_REG_NOMBRE_APELLIDO THEN
                    SET V_DETALLE = CONCAT(V_DETALLE, 'Pide nombre: "', IF(V_NOMBRE_ANTERIOR = 1, 'Sí', 'No'), '" -> "', IF(P_REG_NOMBRE_APELLIDO = 1, 'Sí', 'No'), '"; ');
                END IF;

                IF V_EMPRESA_ANTERIOR <> P_REG_EMPRESA THEN
                    SET V_DETALLE = CONCAT(V_DETALLE, 'Pide empresa: "', IF(V_EMPRESA_ANTERIOR = 1, 'Sí', 'No'), '" -> "', IF(P_REG_EMPRESA = 1, 'Sí', 'No'), '"; ');
                END IF;

                IF V_PATENTE_ANTERIOR <> P_REG_PATENTE THEN
                    SET V_DETALLE = CONCAT(V_DETALLE, 'Pide patente: "', IF(V_PATENTE_ANTERIOR = 1, 'Sí', 'No'), '" -> "', IF(P_REG_PATENTE = 1, 'Sí', 'No'), '"; ');
                END IF;

                IF V_COMENTARIO_ANTERIOR <> P_REG_COMENTARIO THEN
                    SET V_DETALLE = CONCAT(V_DETALLE, 'Pide comentario: "', IF(V_COMENTARIO_ANTERIOR = 1, 'Sí', 'No'), '" -> "', IF(P_REG_COMENTARIO = 1, 'Sí', 'No'), '"; ');
                END IF;

                IF V_FOTOGRAFIA_ANTERIOR <> P_REG_FOTOGRAFIA THEN
                    SET V_DETALLE = CONCAT(V_DETALLE, 'Pide fotografía: "', IF(V_FOTOGRAFIA_ANTERIOR = 1, 'Sí', 'No'), '" -> "', IF(P_REG_FOTOGRAFIA = 1, 'Sí', 'No'), '"; ');
                END IF;

                IF V_ENTRADA_SALIDA_ANTERIOR <> P_REG_ENTRADA_SALIDA THEN
                    SET V_DETALLE = CONCAT(V_DETALLE, 'Indica entrada/salida: "', IF(V_ENTRADA_SALIDA_ANTERIOR = 1, 'Sí', 'No'), '" -> "', IF(P_REG_ENTRADA_SALIDA = 1, 'Sí', 'No'), '"; ');
                END IF;

                IF V_LISTA_ANTERIOR <> P_REG_LISTA_BLANCA_NEGRA THEN
                    SET V_DETALLE = CONCAT(
                        V_DETALLE,
                        'Lista: "',
                        CASE V_LISTA_ANTERIOR WHEN 1 THEN 'Lista blanca' WHEN 2 THEN 'Lista negra' ELSE 'Sin lista' END,
                        '" -> "',
                        CASE P_REG_LISTA_BLANCA_NEGRA WHEN 1 THEN 'Lista blanca' WHEN 2 THEN 'Lista negra' ELSE 'Sin lista' END,
                        '"; '
                    );
                END IF;

                IF V_ELIMINA_ANTERIOR <> P_REG_ELIMINA_ENVIADOS THEN
                    SET V_DETALLE = CONCAT(V_DETALLE, 'Permite eliminar enviados: "', IF(V_ELIMINA_ANTERIOR = 1, 'Sí', 'No'), '" -> "', IF(P_REG_ELIMINA_ENVIADOS = 1, 'Sí', 'No'), '"; ');
                END IF;

                IF V_ESTADO_ANTERIOR <> P_REG_ESTADO THEN
                    SET V_DETALLE = CONCAT(V_DETALLE, 'Estado: "', IF(V_ESTADO_ANTERIOR = 0, 'Vigente', 'Anulado'), '" -> "', IF(P_REG_ESTADO = 0, 'Vigente', 'Anulado'), '"; ');
                END IF;

                IF V_DETALLE <> '' THEN
                    CALL SP_INS_REGLA_BITACORA(P_REG_ID, V_USU_RUT, 'MODIFICAR', 'Regla modificada', V_DETALLE);
                END IF;

                COMMIT;
                SET P_RETORNO = 0;
            END IF;
        END IF;
    END IF;
END$$

DROP PROCEDURE IF EXISTS SP_UPD_REGLA_ITEM$$
CREATE DEFINER=`root`@`%` PROCEDURE SP_UPD_REGLA_ITEM(
    IN P_REG_ID INT UNSIGNED,
    IN P_REGLA_ITEM VARCHAR(20),
    IN P_REGLA_ITEM_VALOR TINYINT UNSIGNED,
    IN P_USU_RUT VARCHAR(10),
    OUT P_RETORNO TINYINT UNSIGNED
)
BEGIN
    DECLARE V_ITEM VARCHAR(20) DEFAULT '';
    DECLARE V_VALOR_ANTERIOR TINYINT UNSIGNED DEFAULT NULL;
    DECLARE V_REGISTRO_EXISTE TINYINT UNSIGNED DEFAULT 0;
    DECLARE V_NOMBRE_ITEM VARCHAR(50) DEFAULT '';
    DECLARE V_DETALLE TEXT DEFAULT '';

    SET V_ITEM = UPPER(TRIM(P_REGLA_ITEM));
    SET P_RETORNO = 0;

    IF V_ITEM NOT IN (
        'NOMBRE', 'EMPRESA', 'PATENTE', 'COMENTARIO', 'FOTOGRAFIA',
        'ENTRADASALIDA', 'LISTABLANCA', 'LISTABLANCA_NEGRA',
        'LISTABLANCANEGRA', 'ELIMINAENVIADOS'
    ) THEN
        SET P_RETORNO = 2;
    ELSEIF V_ITEM IN ('LISTABLANCA', 'LISTABLANCA_NEGRA', 'LISTABLANCANEGRA')
        AND P_REGLA_ITEM_VALOR NOT IN (0, 1, 2) THEN
        SET P_RETORNO = 2;
    ELSEIF V_ITEM NOT IN ('LISTABLANCA', 'LISTABLANCA_NEGRA', 'LISTABLANCANEGRA')
        AND P_REGLA_ITEM_VALOR NOT IN (0, 1) THEN
        SET P_RETORNO = 2;
    ELSE
        START TRANSACTION;

        SELECT COUNT(1) INTO V_REGISTRO_EXISTE
        FROM tbl_regla
        WHERE REG_ID = P_REG_ID;

        IF V_REGISTRO_EXISTE = 0 THEN
            SET P_RETORNO = 1;
        ELSE
            SELECT
                CASE V_ITEM
                    WHEN 'NOMBRE' THEN REG_NOMBRE_APELLIDO
                    WHEN 'EMPRESA' THEN REG_EMPRESA
                    WHEN 'PATENTE' THEN REG_PATENTE
                    WHEN 'COMENTARIO' THEN REG_COMENTARIO
                    WHEN 'FOTOGRAFIA' THEN REG_FOTOGRAFIA
                    WHEN 'ENTRADASALIDA' THEN REG_ENTRADA_SALIDA
                    WHEN 'ELIMINAENVIADOS' THEN REG_ELIMINA_ENVIADOS
                    ELSE REG_LISTA_BLANCA_NEGRA
                END
            INTO V_VALOR_ANTERIOR
            FROM tbl_regla
            WHERE REG_ID = P_REG_ID
            FOR UPDATE;

            SET V_NOMBRE_ITEM = CASE V_ITEM
                WHEN 'NOMBRE' THEN 'Pide nombre'
                WHEN 'EMPRESA' THEN 'Pide empresa'
                WHEN 'PATENTE' THEN 'Pide patente'
                WHEN 'COMENTARIO' THEN 'Pide comentario'
                WHEN 'FOTOGRAFIA' THEN 'Pide fotografía'
                WHEN 'ENTRADASALIDA' THEN 'Indica entrada / salida'
                WHEN 'ELIMINAENVIADOS' THEN 'Permite eliminar enviados'
                ELSE 'Lista blanca / negra'
            END;
        END IF;

        IF P_RETORNO = 0 AND V_VALOR_ANTERIOR <> P_REGLA_ITEM_VALOR THEN
            UPDATE tbl_regla
            SET REG_NOMBRE_APELLIDO = IF(V_ITEM = 'NOMBRE', P_REGLA_ITEM_VALOR, REG_NOMBRE_APELLIDO),
                REG_EMPRESA = IF(V_ITEM = 'EMPRESA', P_REGLA_ITEM_VALOR, REG_EMPRESA),
                REG_PATENTE = IF(V_ITEM = 'PATENTE', P_REGLA_ITEM_VALOR, REG_PATENTE),
                REG_COMENTARIO = IF(V_ITEM = 'COMENTARIO', P_REGLA_ITEM_VALOR, REG_COMENTARIO),
                REG_FOTOGRAFIA = IF(V_ITEM = 'FOTOGRAFIA', P_REGLA_ITEM_VALOR, REG_FOTOGRAFIA),
                REG_ENTRADA_SALIDA = IF(V_ITEM = 'ENTRADASALIDA', P_REGLA_ITEM_VALOR, REG_ENTRADA_SALIDA),
                REG_LISTA_BLANCA_NEGRA = IF(
                    V_ITEM IN ('LISTABLANCA', 'LISTABLANCA_NEGRA', 'LISTABLANCANEGRA'),
                    P_REGLA_ITEM_VALOR,
                    REG_LISTA_BLANCA_NEGRA
                ),
                REG_ELIMINA_ENVIADOS = IF(V_ITEM = 'ELIMINAENVIADOS', P_REGLA_ITEM_VALOR, REG_ELIMINA_ENVIADOS),
                REG_UPDATED_AT = CURRENT_TIMESTAMP(6)
            WHERE REG_ID = P_REG_ID;

            SET V_DETALLE = CONCAT(
                V_NOMBRE_ITEM, ': "',
                CASE V_VALOR_ANTERIOR
                    WHEN 0 THEN 'No'
                    WHEN 1 THEN 'Sí'
                    ELSE 'No aplica'
                END,
                '" -> "',
                CASE P_REGLA_ITEM_VALOR
                    WHEN 0 THEN 'No'
                    WHEN 1 THEN 'Sí'
                    ELSE 'Lista negra'
                END,
                '";'
            );

            CALL SP_INS_REGLA_BITACORA(
                P_REG_ID,
                UPPER(TRIM(P_USU_RUT)),
                'MODIFICAR',
                'Regla modificada',
                V_DETALLE
            );
        END IF;

        COMMIT;
    END IF;
END$$

DELIMITER ;

/* BLOQUE 3 - Corregir el detalle ya registrado en la bitacora. */
UPDATE tbl_regla_bitacora
SET RB_DETALLE = REPLACE(REPLACE(RB_DETALLE, '""""', '"'), '""', '"')
WHERE RB_DETALLE LIKE '%""%';

/* BLOQUE 4 - Verificacion: ambas consultas deben indicar 0. */
SELECT ROUTINE_NAME,
       ROUTINE_DEFINITION LIKE '%""%' AS tiene_comillas_dobles
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = 'dpt_cedula_2'
  AND ROUTINE_NAME IN ('SP_INS_REGLA', 'SP_UPD_REGLA', 'SP_UPD_REGLA_ITEM');

SELECT COUNT(*) AS filas_afectadas
FROM tbl_regla_bitacora
WHERE RB_DETALLE LIKE '%""%';
