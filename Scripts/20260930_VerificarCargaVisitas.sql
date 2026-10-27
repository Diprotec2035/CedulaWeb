/*
    Verificación de 20260930_CargaVisitas.sql en dpt_cedula_2.
    SOLO LECTURA: únicamente SELECT sobre information_schema y tablas.

    PARTE A: ejecutar después de aplicar el script de instalación.
    PARTE B: ejecutar después de las pruebas con los CSV de prueba_carga.
    En cada consulta se indica el resultado esperado.
*/

USE dpt_cedula_2;

/* ======================================================================
   PARTE A - INSTALACIÓN
   ====================================================================== */

/* A1. Tablas. Esperado: 2 filas, ENGINE InnoDB, utf8mb4_0900_ai_ci. */
SELECT TABLE_NAME, ENGINE, TABLE_COLLATION, AUTO_INCREMENT
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('tbl_empresa_visita_carga', 'tbl_empresa_visita_carga_detalle')
ORDER BY TABLE_NAME;

/* A2. Columnas. Esperado: 12 en tbl_empresa_visita_carga y 11 en el detalle. */
SELECT TABLE_NAME, COUNT(*) AS columnas
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('tbl_empresa_visita_carga', 'tbl_empresa_visita_carga_detalle')
GROUP BY TABLE_NAME;

SELECT TABLE_NAME, ORDINAL_POSITION, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('tbl_empresa_visita_carga', 'tbl_empresa_visita_carga_detalle')
ORDER BY TABLE_NAME, ORDINAL_POSITION;

/* A3. Índices. Esperado: carga = PRIMARY + 4 índices; detalle = PRIMARY + UQ carga/fila + 2 índices
       (más el índice que MySQL crea para la clave foránea, si aplica). */
SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE,
       GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX) AS columnas
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('tbl_empresa_visita_carga', 'tbl_empresa_visita_carga_detalle')
GROUP BY TABLE_NAME, INDEX_NAME, NON_UNIQUE
ORDER BY TABLE_NAME, INDEX_NAME;

/* A4. Claves foráneas. Esperado: 3 filas
       (carga -> tbl_empresa, carga -> tbl_usuario, detalle -> carga). */
SELECT TABLE_NAME, CONSTRAINT_NAME, COLUMN_NAME, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('tbl_empresa_visita_carga', 'tbl_empresa_visita_carga_detalle')
  AND REFERENCED_TABLE_NAME IS NOT NULL
ORDER BY TABLE_NAME, CONSTRAINT_NAME;

/* A5. Procedimientos nuevos. Esperado: 6 filas, todas con DEFINER UsuarioOwner@% y SECURITY DEFINER. */
SELECT ROUTINE_NAME, DEFINER, SECURITY_TYPE, CREATED
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_NAME IN (
      'SP_INS_EMPRESA_VISITA_CARGA',
      'SP_INS_EMPRESA_VISITA_CARGA_DETALLE',
      'SP_UPD_EMPRESA_VISITA_CARGA_RESULTADO',
      'SP_SEL_EMPRESA_VISITA_CARGA',
      'SP_SEL_EMPRESA_VISITA_CARGA_ID',
      'SP_SEL_EMPRESA_VISITA_CARGA_DETALLE'
  )
ORDER BY ROUTINE_NAME;

/* A6. Parámetros de los procedimientos nuevos (deben coincidir con la capa Datos).
       Esperado: INS_CARGA 9, INS_CARGA_DETALLE 11, UPD_RESULTADO 6,
                 SEL_CARGA 7, SEL_CARGA_ID 2, SEL_CARGA_DETALLE 5. */
SELECT SPECIFIC_NAME, COUNT(*) AS parametros
FROM information_schema.PARAMETERS
WHERE SPECIFIC_SCHEMA = DATABASE()
  AND SPECIFIC_NAME LIKE 'SP\_%\_EMPRESA\_VISITA\_CARGA%'
GROUP BY SPECIFIC_NAME
ORDER BY SPECIFIC_NAME;

SELECT SPECIFIC_NAME, ORDINAL_POSITION, PARAMETER_MODE, PARAMETER_NAME, DTD_IDENTIFIER
FROM information_schema.PARAMETERS
WHERE SPECIFIC_SCHEMA = DATABASE()
  AND SPECIFIC_NAME LIKE 'SP\_%\_EMPRESA\_VISITA\_CARGA%'
ORDER BY SPECIFIC_NAME, ORDINAL_POSITION;

/* A7. Procedimientos existentes que reutiliza la carga. Esperado: 3 filas.
       SP_DEL_EMPRESA_VISITA sigue existiendo, pero la web ya no lo usa. */
SELECT ROUTINE_NAME, DEFINER
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_NAME IN ('SP_SEL_EMPRESA_VISITA_ESPECIFICO', 'SP_INS_EMPRESA_VISITA',
                       'SP_UPD_EMPRESA_VISITA', 'SP_DEL_EMPRESA_VISITA')
ORDER BY ROUTINE_NAME;

/* A8. Menú VISITAS (CAT_ID = 2). Esperado:
       VISITAS orden 1 visible, CARGAR VISITAS orden 2 visible, REGISTRO VISITAS orden 3 visible;
       VISITAS NUEVO / EDITAR y CARGAR VISITAS NUEVO / DETALLE ocultos (MOD_MENU_VISIBLE = 1).
       Todos con MOD_ESTADO = 0. */
SELECT MOD_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO, MOD_ORDEN, MOD_URL,
       MOD_URL_PUBLICA, MOD_ESTADO, MOD_ADMINISTRADOR, MOD_MENU_VISIBLE
FROM tbl_modulo
WHERE CAT_ID = 2
ORDER BY MOD_MENU_VISIBLE, MOD_ORDEN, MOD_ID;

/* A9. URLs duplicadas. Esperado: 0 filas. */
SELECT MOD_URL, COUNT(*) AS cantidad
FROM tbl_modulo
GROUP BY MOD_URL
HAVING COUNT(*) > 1;

SELECT MOD_URL_PUBLICA, COUNT(*) AS cantidad
FROM tbl_modulo
WHERE MOD_URL_PUBLICA IS NOT NULL
GROUP BY MOD_URL_PUBLICA
HAVING COUNT(*) > 1;

/* A10. Navegación. Esperado: 4 filas vigentes (MN_ESTADO = 0):
        CARGAR VISITAS -NUEVO-> CARGAR VISITAS NUEVO
        CARGAR VISITAS -DETALLE-> CARGAR VISITAS DETALLE
        CARGAR VISITAS NUEVO -VOLVER-> CARGAR VISITAS
        CARGAR VISITAS DETALLE -VOLVER-> CARGAR VISITAS */
SELECT MO.MOD_NOMBRE AS origen, MN.MN_ACCION, MD.MOD_NOMBRE AS destino, MN.MN_ESTADO
FROM tbl_modulo_navegacion MN
INNER JOIN tbl_modulo MO ON MO.MOD_ID = MN.MOD_ID_ORIGEN
INNER JOIN tbl_modulo MD ON MD.MOD_ID = MN.MOD_ID_DESTINO
WHERE MO.MOD_URL LIKE '/aplicacion/visita/frmVisitasImportar%'
ORDER BY MO.MOD_ID, MN.MN_ACCION;

/* A11. Navegación de Visitas (MOD_ID 6) sin cambios. Esperado: NUEVO -> VISITAS NUEVO, EDITAR -> VISITAS EDITAR. */
SELECT MO.MOD_NOMBRE AS origen, MN.MN_ACCION, MD.MOD_NOMBRE AS destino, MN.MN_ESTADO
FROM tbl_modulo_navegacion MN
INNER JOIN tbl_modulo MO ON MO.MOD_ID = MN.MOD_ID_ORIGEN
INNER JOIN tbl_modulo MD ON MD.MOD_ID = MN.MOD_ID_DESTINO
WHERE MN.MOD_ID_ORIGEN = 6
ORDER BY MN.MN_ACCION;

/* A12. Bitácora de los cambios de módulos. Esperado: registros CREAR de los 3 módulos,
        CREAR de las 4 navegaciones y la modificación de los MOD_ID 6 y 8. */
SELECT MB.MB_ID, MB.MOD_ID, MB.MB_FECHA, MB.MB_HORA, MB.USU_RUT, MB.MB_ACCION, MB.MB_COMENTARIOS, MB.MB_DETALLE
FROM tbl_modulo_bitacora MB
WHERE MB.MOD_ID IN (
        SELECT MOD_ID FROM tbl_modulo WHERE MOD_URL LIKE '/aplicacion/visita/frmVisitasImportar%'
    )
   OR (MB.MOD_ID IN (6, 8) AND MB.MB_FECHA = CURRENT_DATE)
ORDER BY MB.MB_ID;

/* A13. Permisos asignados al módulo CARGAR VISITAS (Módulos x perfil).
        Esperado: una fila por perfil que deba cargar visitas, con MP_ESTADO = 0 y MP_NUEVO = 0.
        0 filas = nadie (salvo super administrador) verá el módulo todavía. */
SELECT PER.EMP_RUT, PER.PER_NOMBRE, MP.MP_ESTADO, MP.MP_CONSULTA, MP.MP_NUEVO, MP.MP_MODIFICA
FROM tbl_modulo_perfil MP
INNER JOIN tbl_perfil PER ON PER.PER_ID = MP.PER_ID
INNER JOIN tbl_modulo MODU ON MODU.MOD_ID = MP.MOD_ID
WHERE MODU.MOD_URL = '/aplicacion/visita/frmVisitasImportar.aspx'
ORDER BY PER.EMP_RUT, PER.PER_NOMBRE;

/* ======================================================================
   PARTE B - RESULTADOS DE LAS PRUEBAS
   Reemplazar el RUT de la empresa donde se hicieron las pruebas.
   ====================================================================== */

SET @emp_rut = 'REEMPLAZAR';

/* B1. Cargas realizadas (más reciente primero).
       Esperado: una fila por archivo aceptado (01-09, 15, 16), ninguna para 10-14. */
SELECT EVC.EVC_ID, EVC.EVC_ARCHIVO_NOMBRE, EVC.EVC_TOTAL_FILAS, EVC.EVC_TOTAL_OK, EVC.EVC_TOTAL_ERROR,
       CASE EVC.EVC_ESTADO WHEN 0 THEN 'Finalizada' WHEN 1 THEN 'Anulada' WHEN 2 THEN 'Procesando'
            WHEN 3 THEN 'Finalizada con errores' WHEN 4 THEN 'Fallida' END AS estado,
       EVC.EVC_CREATED_AT, EVC.EVC_CREATED_BY, EVC.EVC_CREATED_BY_NOMBRE
FROM tbl_empresa_visita_carga EVC
WHERE EVC.EMP_RUT = @emp_rut
ORDER BY EVC.EVC_ID DESC;

/* B2. Coherencia entre totales y detalle. Esperado: 0 filas.
       Una fila aquí indica que el detalle no coincide con los totales guardados. */
SELECT EVC.EVC_ID, EVC.EVC_ARCHIVO_NOMBRE,
       EVC.EVC_TOTAL_FILAS, COUNT(EVCD.EVCD_ID) AS filas_detalle,
       EVC.EVC_TOTAL_OK, SUM(EVCD.EVCD_ESTADO = 0) AS ok_detalle,
       EVC.EVC_TOTAL_ERROR, SUM(EVCD.EVCD_ESTADO = 1) AS error_detalle
FROM tbl_empresa_visita_carga EVC
LEFT JOIN tbl_empresa_visita_carga_detalle EVCD ON EVCD.EVC_ID = EVC.EVC_ID
WHERE EVC.EMP_RUT = @emp_rut
GROUP BY EVC.EVC_ID, EVC.EVC_ARCHIVO_NOMBRE, EVC.EVC_TOTAL_FILAS, EVC.EVC_TOTAL_OK, EVC.EVC_TOTAL_ERROR
HAVING EVC.EVC_TOTAL_FILAS <> COUNT(EVCD.EVCD_ID)
    OR EVC.EVC_TOTAL_OK <> IFNULL(SUM(EVCD.EVCD_ESTADO = 0), 0)
    OR EVC.EVC_TOTAL_ERROR <> IFNULL(SUM(EVCD.EVCD_ESTADO = 1), 0);

/* B3. Cargas que quedaron en Procesando o Fallida. Esperado: 0 filas. */
SELECT EVC_ID, EVC_ARCHIVO_NOMBRE, EVC_ESTADO, EVC_CREATED_AT
FROM tbl_empresa_visita_carga
WHERE EMP_RUT = @emp_rut
  AND EVC_ESTADO IN (2, 4);

/* B4. Resumen de acciones por carga (Insertada / Actualizada / Sin cambios / Error). */
SELECT EVC.EVC_ID, EVC.EVC_ARCHIVO_NOMBRE,
       SUM(EVCD.EVCD_ACCION = 1) AS insertadas,
       SUM(EVCD.EVCD_ACCION = 2) AS actualizadas,
       SUM(EVCD.EVCD_ACCION = 3) AS sin_cambios,
       SUM(EVCD.EVCD_ESTADO = 1) AS errores
FROM tbl_empresa_visita_carga EVC
INNER JOIN tbl_empresa_visita_carga_detalle EVCD ON EVCD.EVC_ID = EVC.EVC_ID
WHERE EVC.EMP_RUT = @emp_rut
GROUP BY EVC.EVC_ID, EVC.EVC_ARCHIVO_NOMBRE
ORDER BY EVC.EVC_ID;

/* B5. Coherencia de acción y estado. Esperado: 0 filas.
       Error debe tener acción 0; Registrado debe tener acción 1, 2 o 3. */
SELECT EVCD.EVC_ID, EVCD.EVCD_FILA, EVCD.EVCD_ACCION, EVCD.EVCD_ESTADO, EVCD.EVCD_MENSAJE
FROM tbl_empresa_visita_carga_detalle EVCD
INNER JOIN tbl_empresa_visita_carga EVC ON EVC.EVC_ID = EVCD.EVC_ID
WHERE EVC.EMP_RUT = @emp_rut
  AND ((EVCD.EVCD_ESTADO = 1 AND EVCD.EVCD_ACCION <> 0)
    OR (EVCD.EVCD_ESTADO = 0 AND EVCD.EVCD_ACCION NOT IN (1, 2, 3)));

/* B6. Valores anteriores solo en filas actualizadas. Esperado: 0 filas. */
SELECT EVCD.EVC_ID, EVCD.EVCD_FILA, EVCD.EVCD_ACCION, EVCD.EV_NOMBRE_ANTERIOR, EVCD.EV_EMPRESA_ANTERIOR
FROM tbl_empresa_visita_carga_detalle EVCD
INNER JOIN tbl_empresa_visita_carga EVC ON EVC.EVC_ID = EVCD.EVC_ID
WHERE EVC.EMP_RUT = @emp_rut
  AND EVCD.EVCD_ACCION <> 2
  AND (EVCD.EV_NOMBRE_ANTERIOR IS NOT NULL OR EVCD.EV_EMPRESA_ANTERIOR IS NOT NULL);

/* B7. Detalle de las filas actualizadas (archivos 03 y 15): antes y después. */
SELECT EVC.EVC_ARCHIVO_NOMBRE, EVCD.EVCD_FILA, EVCD.EV_RUT,
       EVCD.EV_NOMBRE_ANTERIOR, EVCD.EV_NOMBRE,
       EVCD.EV_EMPRESA_ANTERIOR, EVCD.EV_EMPRESA, EVCD.EVCD_MENSAJE
FROM tbl_empresa_visita_carga_detalle EVCD
INNER JOIN tbl_empresa_visita_carga EVC ON EVC.EVC_ID = EVCD.EVC_ID
WHERE EVC.EMP_RUT = @emp_rut
  AND EVCD.EVCD_ACCION = 2
ORDER BY EVC.EVC_ID, EVCD.EVCD_FILA;

/* B8. Filas con error y su motivo (archivos 04 y 05). */
SELECT EVC.EVC_ARCHIVO_NOMBRE, EVCD.EVCD_FILA, EVCD.EV_RUT, EVCD.EVCD_MENSAJE
FROM tbl_empresa_visita_carga_detalle EVCD
INNER JOIN tbl_empresa_visita_carga EVC ON EVC.EVC_ID = EVCD.EVC_ID
WHERE EVC.EMP_RUT = @emp_rut
  AND EVCD.EVCD_ESTADO = 1
ORDER BY EVC.EVC_ID, EVCD.EVCD_FILA;

/* B9. Visitas de prueba registradas. Esperado (tras 01 al 09 y 15):
       30100001-4 al 30100007-3, 30100009-K, 30100010-3, 30100107-K,
       30100201-7 al 30100203-3, 30100301-3 al 30100303-K,
       30100401-K, 30100402-8, 30100501-6, 30100502-4.
       30100005-7 debe tener EV_VIGENTE = 1 si se dio de baja antes del archivo 15. */
SELECT EV_RUT, EV_NOMBRE, EV_EMPRESA, EV_VIGENTE, EV_FECHA, EV_HORA
FROM tbl_empresa_visita
WHERE EMP_RUT = @emp_rut
  AND EV_RUT LIKE '301%'
ORDER BY EV_RUT;

/* B10. Visitas del archivo masivo. Esperado: 1000. */
SELECT COUNT(*) AS visitas_masivas
FROM tbl_empresa_visita
WHERE EMP_RUT = @emp_rut
  AND EV_RUT LIKE '302%';

/* B11. Formato de RUT guardado. Esperado: 0 filas
        (sin puntos, sin espacios, con guion y K en mayúscula). */
SELECT EV_RUT, EV_NOMBRE
FROM tbl_empresa_visita
WHERE EMP_RUT = @emp_rut
  AND (EV_RUT LIKE '%.%' OR EV_RUT LIKE '% %' OR EV_RUT NOT LIKE '%-%'
       OR BINARY EV_RUT LIKE BINARY '%k');

/* B12. Tildes y eñes dañadas (archivos 06, 07 y 08). Esperado: 0 filas.
        Se compara en binario porque la colación de la tabla no distingue acentos. */
SELECT EV_RUT, EV_NOMBRE, EV_EMPRESA
FROM tbl_empresa_visita
WHERE EMP_RUT = @emp_rut
  AND (EV_NOMBRE COLLATE utf8mb4_bin LIKE '%Ã%'
       OR EV_EMPRESA COLLATE utf8mb4_bin LIKE '%Ã%'
       OR EV_NOMBRE COLLATE utf8mb4_bin LIKE CONCAT('%', CHAR(0xEFBFBD USING utf8mb4), '%')
       OR EV_EMPRESA COLLATE utf8mb4_bin LIKE CONCAT('%', CHAR(0xEFBFBD USING utf8mb4), '%')
       OR EV_NOMBRE LIKE '%?%'
       OR EV_EMPRESA LIKE '%?%');

/* B12b. Nombres con tildes y eñes de los archivos 06, 07 y 08, para revisarlos a simple vista.
         Esperado: ÍÑIGO ÑÚÑEZ PEÑA, COMPAÑÍA MINERA ÑANDÚ, INGENIERÍA, MONTAJE Y OBRAS, etc. */
SELECT EV_RUT, EV_NOMBRE, EV_EMPRESA
FROM tbl_empresa_visita
WHERE EMP_RUT = @emp_rut
  AND EV_RUT IN ('30100201-7', '30100202-5', '30100203-3',
                 '30100301-3', '30100302-1', '30100303-K',
                 '30100401-K', '30100402-8')
ORDER BY EV_RUT;

/* B13. RUT repetidos en la lista. Esperado: 0 filas (lo garantiza la llave primaria). */
SELECT EV_RUT, COUNT(*) AS cantidad
FROM tbl_empresa_visita
WHERE EMP_RUT = @emp_rut
GROUP BY EV_RUT
HAVING COUNT(*) > 1;

/* B14. Visitas que existían antes de las pruebas siguen presentes.
        Comparar este total con el que tenía la empresa antes de probar:
        debe ser igual o mayor (la carga nunca elimina). */
SELECT COUNT(*) AS total_visitas,
       SUM(EV_VIGENTE = 0) AS vigentes,
       SUM(EV_VIGENTE = 1) AS no_vigentes,
       SUM(EV_RUT NOT LIKE '301%' AND EV_RUT NOT LIKE '302%') AS visitas_no_de_prueba
FROM tbl_empresa_visita
WHERE EMP_RUT = @emp_rut;
