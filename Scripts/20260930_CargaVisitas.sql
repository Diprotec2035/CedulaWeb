/*
    Carga masiva de visitas (Cargar visitas) para dpt_cedula_2.

    Replica el patrón de Importar productos de dpt_inventario:
    tbl_producto_carga / tbl_producto_carga_detalle y sus procedimientos.

    Diferencias intencionales con Importar productos:
    - El RUT existente no se rechaza: se actualizan nombre y empresa,
      conservando la vigencia (se reutiliza SP_UPD_EMPRESA_VISITA).
    - El detalle guarda la acción aplicada y los valores anteriores.
    - Las columnas del detalle son más holgadas que tbl_empresa_visita,
      para registrar como error filas con valores demasiado largos.
    - SP_SEL_EMPRESA_VISITA_CARGA_DETALLE recibe también EMP_RUT.

    Ejecutar POR BLOQUES, conectado como root@% (tiene SET_ANY_DEFINER),
    con respaldo previo. El DDL de MySQL hace COMMIT implícito.
    No modifica tbl_empresa_visita ni sus procedimientos existentes.
*/

USE dpt_cedula_2;

/* ======================================================================
   BLOQUE 0 - Comprobaciones previas (solo lectura)
   Esperado: base_actual = dpt_cedula_2, tablas_existentes = 0,
   procedimientos_existentes = 0, modulos_existentes = 0.
   ====================================================================== */

SELECT DATABASE() AS base_actual;

SELECT COUNT(*) AS tablas_existentes
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('tbl_empresa_visita_carga', 'tbl_empresa_visita_carga_detalle');

SELECT COUNT(*) AS procedimientos_existentes
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_NAME LIKE 'SP\_%\_EMPRESA\_VISITA\_CARGA%';

SELECT COUNT(*) AS modulos_existentes
FROM tbl_modulo
WHERE MOD_URL LIKE '/aplicacion/visita/frmVisitasImportar%';

SELECT MOD_ID, MOD_NOMBRE, MOD_ORDEN, MOD_URL
FROM tbl_modulo
WHERE CAT_ID = 2
ORDER BY MOD_ORDEN, MOD_ID;

/* ======================================================================
   BLOQUE 1 - Tablas
   ====================================================================== */

CREATE TABLE `tbl_empresa_visita_carga` (
  `EVC_ID` INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
  `EMP_RUT` VARCHAR(10) COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EVC_ARCHIVO_NOMBRE` VARCHAR(255) COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EVC_ARCHIVO_RUTA` VARCHAR(500) COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EVC_TOTAL_FILAS` INTEGER UNSIGNED NOT NULL DEFAULT 0,
  `EVC_TOTAL_OK` INTEGER UNSIGNED NOT NULL DEFAULT 0,
  `EVC_TOTAL_ERROR` INTEGER UNSIGNED NOT NULL DEFAULT 0,
  `EVC_ESTADO` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `EVC_CREATED_AT` DATETIME(6) NOT NULL,
  `EVC_CREATED_BY` VARCHAR(10) COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EVC_CREATED_BY_NOMBRE` VARCHAR(100) COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EVC_UPDATED_AT` DATETIME(6) NOT NULL DEFAULT (CURRENT_TIMESTAMP(6)),
  PRIMARY KEY USING BTREE (`EVC_ID`),
  KEY `IX_EMPRESA_VISITA_CARGA_ESTADO` USING BTREE (`EVC_ESTADO`),
  KEY `IX_EMPRESA_VISITA_CARGA_FECHA` USING BTREE (`EVC_CREATED_AT`),
  KEY `IX_EMPRESA_VISITA_CARGA_CREATED_BY` USING BTREE (`EVC_CREATED_BY`),
  KEY `IX_EMPRESA_VISITA_CARGA_EMPRESA_ESTADO_FECHA` USING BTREE (`EMP_RUT`, `EVC_ESTADO`, `EVC_CREATED_AT`),
  CONSTRAINT `FK_EMPRESA_VISITA_CARGA:CREATED_BY` FOREIGN KEY (`EVC_CREATED_BY`) REFERENCES `tbl_usuario` (`USU_RUT`),
  CONSTRAINT `FK_EMPRESA_VISITA_CARGA:EMPRESA` FOREIGN KEY (`EMP_RUT`) REFERENCES `tbl_empresa` (`EMP_RUT`)
) ENGINE=InnoDB
ROW_FORMAT=DYNAMIC CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_0900_ai_ci';

/*
    EVCD_ACCION: 0 = Sin aplicar (fila con error), 1 = Insertada,
                 2 = Actualizada, 3 = Sin cambios.
    EVCD_ESTADO: 0 = Registrado, 1 = Error (igual que Importar productos).
    EV_NOMBRE_ANTERIOR / EV_EMPRESA_ANTERIOR: solo se informan cuando
    la fila actualizó ese dato.
*/
CREATE TABLE `tbl_empresa_visita_carga_detalle` (
  `EVCD_ID` INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
  `EVC_ID` INTEGER UNSIGNED NOT NULL,
  `EVCD_FILA` INTEGER UNSIGNED NOT NULL,
  `EV_RUT` VARCHAR(50) COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EV_NOMBRE` VARCHAR(255) COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EV_EMPRESA` VARCHAR(255) COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EVCD_ACCION` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `EV_NOMBRE_ANTERIOR` VARCHAR(150) COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EV_EMPRESA_ANTERIOR` VARCHAR(50) COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EVCD_ESTADO` TINYINT UNSIGNED NOT NULL,
  `EVCD_MENSAJE` VARCHAR(1000) COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY USING BTREE (`EVCD_ID`),
  UNIQUE KEY `UQ_EMPRESA_VISITA_CARGA_DETALLE_CARGA_FILA` USING BTREE (`EVC_ID`, `EVCD_FILA`),
  KEY `IX_EMPRESA_VISITA_CARGA_DETALLE_ESTADO` USING BTREE (`EVCD_ESTADO`),
  KEY `IX_EMPRESA_VISITA_CARGA_DETALLE_ACCION` USING BTREE (`EVCD_ACCION`),
  CONSTRAINT `FK_EMPRESA_VISITA_CARGA_DETALLE_CARGA` FOREIGN KEY (`EVC_ID`) REFERENCES `tbl_empresa_visita_carga` (`EVC_ID`)
) ENGINE=InnoDB
ROW_FORMAT=DYNAMIC CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_0900_ai_ci';

/* ======================================================================
   BLOQUE 2 - Procedimientos (ejecutar el bloque completo)
   ====================================================================== */

DELIMITER $$

CREATE DEFINER = 'UsuarioOwner'@'%' PROCEDURE `SP_INS_EMPRESA_VISITA_CARGA`(
        IN `P_EMP_RUT` VARCHAR(10),
        IN `P_EVC_ARCHIVO_NOMBRE` VARCHAR(255),
        IN `P_EVC_ARCHIVO_RUTA` VARCHAR(500),
        IN `P_EVC_ESTADO` TINYINT UNSIGNED,
        IN `P_EVC_CREATED_AT` DATETIME(6),
        IN `P_EVC_CREATED_BY` VARCHAR(10),
        IN `P_EVC_CREATED_BY_NOMBRE` VARCHAR(100),
        OUT `P_EVC_ID` INTEGER UNSIGNED,
        OUT `P_RETORNO` TINYINT UNSIGNED
    )
    NOT DETERMINISTIC
    CONTAINS SQL
    SQL SECURITY DEFINER
    COMMENT ''
BEGIN
    SET P_EVC_ID = 0;

    START TRANSACTION;

    INSERT INTO tbl_empresa_visita_carga
    (
        EMP_RUT,
        EVC_ARCHIVO_NOMBRE,
        EVC_ARCHIVO_RUTA,
        EVC_TOTAL_FILAS,
        EVC_TOTAL_OK,
        EVC_TOTAL_ERROR,
        EVC_ESTADO,
        EVC_CREATED_AT,
        EVC_CREATED_BY,
        EVC_CREATED_BY_NOMBRE,
        EVC_UPDATED_AT
    )
    VALUES
    (
        UPPER(TRIM(P_EMP_RUT)),
        P_EVC_ARCHIVO_NOMBRE,
        P_EVC_ARCHIVO_RUTA,
        0,
        0,
        0,
        P_EVC_ESTADO,
        P_EVC_CREATED_AT,
        UPPER(TRIM(P_EVC_CREATED_BY)),
        P_EVC_CREATED_BY_NOMBRE,
        CURRENT_TIMESTAMP(6)
    );

    SET P_EVC_ID = LAST_INSERT_ID();

    COMMIT;

    SET P_RETORNO = 0;
END$$

CREATE DEFINER = 'UsuarioOwner'@'%' PROCEDURE `SP_INS_EMPRESA_VISITA_CARGA_DETALLE`(
        IN `P_EVC_ID` INTEGER UNSIGNED,
        IN `P_EVCD_FILA` INTEGER UNSIGNED,
        IN `P_EV_RUT` VARCHAR(50),
        IN `P_EV_NOMBRE` VARCHAR(255),
        IN `P_EV_EMPRESA` VARCHAR(255),
        IN `P_EVCD_ACCION` TINYINT UNSIGNED,
        IN `P_EV_NOMBRE_ANTERIOR` VARCHAR(150),
        IN `P_EV_EMPRESA_ANTERIOR` VARCHAR(50),
        IN `P_EVCD_ESTADO` TINYINT UNSIGNED,
        IN `P_EVCD_MENSAJE` VARCHAR(1000),
        OUT `P_RETORNO` TINYINT
    )
    NOT DETERMINISTIC
    CONTAINS SQL
    SQL SECURITY DEFINER
    COMMENT ''
BEGIN
    START TRANSACTION;

    INSERT INTO tbl_empresa_visita_carga_detalle
    (
        EVC_ID,
        EVCD_FILA,
        EV_RUT,
        EV_NOMBRE,
        EV_EMPRESA,
        EVCD_ACCION,
        EV_NOMBRE_ANTERIOR,
        EV_EMPRESA_ANTERIOR,
        EVCD_ESTADO,
        EVCD_MENSAJE
    )
    VALUES
    (
        P_EVC_ID,
        P_EVCD_FILA,
        UPPER(TRIM(P_EV_RUT)),
        UPPER(TRIM(P_EV_NOMBRE)),
        UPPER(TRIM(P_EV_EMPRESA)),
        P_EVCD_ACCION,
        UPPER(TRIM(P_EV_NOMBRE_ANTERIOR)),
        UPPER(TRIM(P_EV_EMPRESA_ANTERIOR)),
        P_EVCD_ESTADO,
        P_EVCD_MENSAJE
    );

    COMMIT;

    SET P_RETORNO = 0;
END$$

CREATE DEFINER = 'UsuarioOwner'@'%' PROCEDURE `SP_UPD_EMPRESA_VISITA_CARGA_RESULTADO`(
        IN `P_EVC_ID` INTEGER UNSIGNED,
        IN `P_EVC_TOTAL_FILAS` INTEGER UNSIGNED,
        IN `P_EVC_TOTAL_OK` INTEGER UNSIGNED,
        IN `P_EVC_TOTAL_ERROR` INTEGER UNSIGNED,
        IN `P_EVC_ESTADO` TINYINT UNSIGNED,
        OUT `P_RETORNO` TINYINT UNSIGNED
    )
    NOT DETERMINISTIC
    CONTAINS SQL
    SQL SECURITY DEFINER
    COMMENT ''
BEGIN
    START TRANSACTION;

    UPDATE
        tbl_empresa_visita_carga
    SET
        EVC_TOTAL_FILAS = P_EVC_TOTAL_FILAS,
        EVC_TOTAL_OK = P_EVC_TOTAL_OK,
        EVC_TOTAL_ERROR = P_EVC_TOTAL_ERROR,
        EVC_ESTADO = P_EVC_ESTADO,
        EVC_UPDATED_AT = CURRENT_TIMESTAMP(6)
    WHERE
        EVC_ID = P_EVC_ID;

    IF ROW_COUNT() = 0 THEN
        SET P_RETORNO = 1;
    ELSE
        SET P_RETORNO = 0;
    END IF;

    COMMIT;
END$$

CREATE DEFINER = 'UsuarioOwner'@'%' PROCEDURE `SP_SEL_EMPRESA_VISITA_CARGA`(
        IN `P_EMP_RUT` VARCHAR(10),
        IN `P_BUSCADOR` VARCHAR(100),
        IN `P_EVC_ESTADO` TINYINT UNSIGNED,
        IN `P_EVC_CREATED_AT_DESDE` VARCHAR(10),
        IN `P_EVC_CREATED_AT_HASTA` VARCHAR(10),
        IN `P_CAMPO` TINYINT,
        IN `P_ORDEN` TINYINT
    )
    NOT DETERMINISTIC
    CONTAINS SQL
    SQL SECURITY DEFINER
    COMMENT ''
BEGIN
    SELECT
        EVC.EVC_ID,
        EVC.EMP_RUT,
        EVC.EVC_ARCHIVO_NOMBRE,
        EVC.EVC_ARCHIVO_RUTA,
        EVC.EVC_TOTAL_FILAS,
        EVC.EVC_TOTAL_OK,
        EVC.EVC_TOTAL_ERROR,
        EVC.EVC_ESTADO,
        CASE EVC.EVC_ESTADO
            WHEN 0 THEN 'Finalizada'
            WHEN 1 THEN 'Anulada'
            WHEN 2 THEN 'Procesando'
            WHEN 3 THEN 'Finalizada con errores'
            WHEN 4 THEN 'Fallida'
            ELSE 'Desconocido'
        END AS EVC_ESTADO_NOMBRE,
        DATE_FORMAT(EVC.EVC_CREATED_AT, '%d-%m-%Y') AS EVC_CREATED_FECHA,
        DATE_FORMAT(EVC.EVC_CREATED_AT, '%H:%i:%s') AS EVC_CREATED_HORA,
        EVC.EVC_CREATED_BY,
        CONCAT(IFNULL(USU.USU_NOMBRE, ''), ' ', IFNULL(USU.USU_APELLIDO, '')) AS EVC_CREATED_BY_NOMBRE
    FROM
        tbl_empresa_visita_carga EVC
        LEFT JOIN
        tbl_usuario USU
        ON
        USU.USU_RUT = EVC.EVC_CREATED_BY
    WHERE
        EVC.EMP_RUT = UPPER(TRIM(P_EMP_RUT))
        AND
        (
            P_BUSCADOR = ''
            OR EVC.EVC_ARCHIVO_NOMBRE LIKE CONCAT('%', P_BUSCADOR, '%')
            OR EVC.EVC_CREATED_BY LIKE CONCAT('%', P_BUSCADOR, '%')
            OR CONCAT(IFNULL(USU.USU_NOMBRE, ''), ' ', IFNULL(USU.USU_APELLIDO, '')) LIKE CONCAT('%', P_BUSCADOR, '%')
        )
        AND
        (
            P_EVC_ESTADO = 9
            OR EVC.EVC_ESTADO = P_EVC_ESTADO
        )
        AND
        (
            P_EVC_CREATED_AT_DESDE = ''
            OR EVC.EVC_CREATED_AT >= STR_TO_DATE(P_EVC_CREATED_AT_DESDE, '%d-%m-%Y')
        )
        AND
        (
            P_EVC_CREATED_AT_HASTA = ''
            OR EVC.EVC_CREATED_AT < DATE_ADD(STR_TO_DATE(P_EVC_CREATED_AT_HASTA, '%d-%m-%Y'), INTERVAL 1 DAY)
        )
    ORDER BY
        CASE WHEN P_CAMPO = 1 AND P_ORDEN = 1 THEN EVC.EVC_CREATED_AT END ASC,
        CASE WHEN P_CAMPO = 1 AND P_ORDEN = 2 THEN EVC.EVC_CREATED_AT END DESC,
        CASE WHEN P_CAMPO = 2 AND P_ORDEN = 1 THEN EVC.EVC_ARCHIVO_NOMBRE END ASC,
        CASE WHEN P_CAMPO = 2 AND P_ORDEN = 2 THEN EVC.EVC_ARCHIVO_NOMBRE END DESC,
        CASE WHEN P_CAMPO = 3 AND P_ORDEN = 1 THEN CONCAT(IFNULL(USU.USU_NOMBRE, ''), ' ', IFNULL(USU.USU_APELLIDO, '')) END ASC,
        CASE WHEN P_CAMPO = 3 AND P_ORDEN = 2 THEN CONCAT(IFNULL(USU.USU_NOMBRE, ''), ' ', IFNULL(USU.USU_APELLIDO, '')) END DESC,
        CASE WHEN P_CAMPO = 4 AND P_ORDEN = 1 THEN EVC.EVC_ESTADO END ASC,
        CASE WHEN P_CAMPO = 4 AND P_ORDEN = 2 THEN EVC.EVC_ESTADO END DESC,
        EVC.EVC_CREATED_AT DESC;
END$$

CREATE DEFINER = 'UsuarioOwner'@'%' PROCEDURE `SP_SEL_EMPRESA_VISITA_CARGA_ID`(
        IN `P_EVC_ID` INTEGER UNSIGNED,
        IN `P_EMP_RUT` VARCHAR(10)
    )
    NOT DETERMINISTIC
    CONTAINS SQL
    SQL SECURITY DEFINER
    COMMENT ''
BEGIN
    SELECT
        EVC.EVC_ID,
        EVC.EMP_RUT,
        EVC.EVC_ARCHIVO_NOMBRE,
        EVC.EVC_ARCHIVO_RUTA,
        EVC.EVC_TOTAL_FILAS,
        EVC.EVC_TOTAL_OK,
        EVC.EVC_TOTAL_ERROR,
        EVC.EVC_ESTADO,
        CASE EVC.EVC_ESTADO
            WHEN 0 THEN 'Finalizada'
            WHEN 1 THEN 'Anulada'
            WHEN 2 THEN 'Procesando'
            WHEN 3 THEN 'Finalizada con errores'
            WHEN 4 THEN 'Fallida'
            ELSE 'Desconocido'
        END AS EVC_ESTADO_NOMBRE,
        DATE_FORMAT(EVC.EVC_CREATED_AT, '%d-%m-%Y') AS EVC_CREATED_FECHA,
        DATE_FORMAT(EVC.EVC_CREATED_AT, '%H:%i:%s') AS EVC_CREATED_HORA,
        EVC.EVC_CREATED_BY,
        EVC.EVC_CREATED_BY_NOMBRE
    FROM
        tbl_empresa_visita_carga EVC
    WHERE
        EVC.EVC_ID = P_EVC_ID
        AND EVC.EMP_RUT = UPPER(TRIM(P_EMP_RUT));
END$$

CREATE DEFINER = 'UsuarioOwner'@'%' PROCEDURE `SP_SEL_EMPRESA_VISITA_CARGA_DETALLE`(
        IN `P_EVC_ID` INTEGER UNSIGNED,
        IN `P_EMP_RUT` VARCHAR(10),
        IN `P_EVCD_ESTADO` TINYINT UNSIGNED,
        IN `P_EVCD_ACCION` TINYINT UNSIGNED,
        IN `P_BUSCADOR` VARCHAR(100)
    )
    NOT DETERMINISTIC
    CONTAINS SQL
    SQL SECURITY DEFINER
    COMMENT ''
BEGIN
    SELECT
        EVCD.EVCD_ID,
        EVCD.EVC_ID,
        EVCD.EVCD_FILA,
        EVCD.EV_RUT,
        EVCD.EV_NOMBRE,
        EVCD.EV_EMPRESA,
        EVCD.EVCD_ACCION,
        CASE EVCD.EVCD_ACCION
            WHEN 0 THEN 'Sin aplicar'
            WHEN 1 THEN 'Insertada'
            WHEN 2 THEN 'Actualizada'
            WHEN 3 THEN 'Sin cambios'
            ELSE 'Desconocido'
        END AS EVCD_ACCION_NOMBRE,
        EVCD.EV_NOMBRE_ANTERIOR,
        EVCD.EV_EMPRESA_ANTERIOR,
        EVCD.EVCD_ESTADO,
        CASE EVCD.EVCD_ESTADO
            WHEN 0 THEN 'Registrado'
            WHEN 1 THEN 'Error'
            ELSE 'Desconocido'
        END AS EVCD_ESTADO_NOMBRE,
        EVCD.EVCD_MENSAJE
    FROM
        tbl_empresa_visita_carga_detalle EVCD
        INNER JOIN
        tbl_empresa_visita_carga EVC
        ON
        EVC.EVC_ID = EVCD.EVC_ID
    WHERE
        EVCD.EVC_ID = P_EVC_ID
        AND EVC.EMP_RUT = UPPER(TRIM(P_EMP_RUT))
        AND
        (
            P_EVCD_ESTADO = 9
            OR EVCD.EVCD_ESTADO = P_EVCD_ESTADO
        )
        AND
        (
            P_EVCD_ACCION = 9
            OR EVCD.EVCD_ACCION = P_EVCD_ACCION
        )
        AND
        (
            P_BUSCADOR = ''
            OR EVCD.EV_RUT LIKE CONCAT('%', P_BUSCADOR, '%')
            OR EVCD.EV_NOMBRE LIKE CONCAT('%', P_BUSCADOR, '%')
            OR EVCD.EV_EMPRESA LIKE CONCAT('%', P_BUSCADOR, '%')
            OR EVCD.EV_NOMBRE_ANTERIOR LIKE CONCAT('%', P_BUSCADOR, '%')
            OR EVCD.EV_EMPRESA_ANTERIOR LIKE CONCAT('%', P_BUSCADOR, '%')
            OR EVCD.EVCD_MENSAJE LIKE CONCAT('%', P_BUSCADOR, '%')
        )
    ORDER BY
        EVCD.EVCD_FILA ASC;
END$$

DELIMITER ;

/* Comprobación: deben aparecer 6 procedimientos con DEFINER UsuarioOwner@%. */
SELECT ROUTINE_NAME, DEFINER
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = DATABASE()
  AND ROUTINE_NAME LIKE 'SP\_%\_EMPRESA\_VISITA\_CARGA%'
ORDER BY ROUTINE_NAME;

/* ======================================================================
   BLOQUE 3 - Módulos del menú (categoría VISITAS, CAT_ID = 2)
   Usa SP_INS_MODULO, que deja bitácora. Reemplazar @usu_rut solo si el
   responsable real del cambio es otro usuario.
   Convención: MOD_ADMINISTRADOR 1 = No, MOD_MENU_VISIBLE 0 = Sí / 1 = No,
   MOD_BITACORA_ESTADO 1 = No (igual que PRODUCTOS IMPORTAR en Inventario).
   ====================================================================== */

SET @usu_rut = '1-9';

SET @retorno = NULL;
CALL SP_INS_MODULO(2, 'CARGAR VISITAS', 'Cargar visitas', 'Listado de cargas de visitas',
    2, '/aplicacion/visita/frmVisitasImportar.aspx', 'vcg-k4t8pz',
    0, 1, 0, 1, NULL, NULL, @usu_rut, @retorno);
SELECT 'CARGAR VISITAS' AS modulo, @retorno AS retorno;

SET @retorno = NULL;
CALL SP_INS_MODULO(2, 'CARGAR VISITAS NUEVO', 'Nueva carga de visitas', 'Datos de la carga',
    99, '/aplicacion/visita/frmVisitasImportarNuevo.aspx', 'vcg-new-r7m2qa',
    0, 1, 1, 1, NULL, NULL, @usu_rut, @retorno);
SELECT 'CARGAR VISITAS NUEVO' AS modulo, @retorno AS retorno;

SET @retorno = NULL;
CALL SP_INS_MODULO(2, 'CARGAR VISITAS DETALLE', 'Detalle carga de visitas', 'Resultado por fila',
    99, '/aplicacion/visita/frmVisitasImportarDetalle.aspx', 'vcg-det-x5n9lc',
    0, 1, 1, 1, NULL, NULL, @usu_rut, @retorno);
SELECT 'CARGAR VISITAS DETALLE' AS modulo, @retorno AS retorno;

SELECT MOD_ID, MOD_NOMBRE, MOD_URL, MOD_URL_PUBLICA, MOD_MENU_VISIBLE
FROM tbl_modulo
WHERE MOD_URL LIKE '/aplicacion/visita/frmVisitasImportar%'
ORDER BY MOD_ID;

/* ======================================================================
   BLOQUE 4 - Navegación entre páginas (igual que PRODUCTOS IMPORTAR)
   Listado -> NUEVO / DETALLE; Nuevo y Detalle -> VOLVER al listado.
   ====================================================================== */

SET @usu_rut = '1-9';

SELECT MOD_ID INTO @mod_carga FROM tbl_modulo WHERE MOD_URL = '/aplicacion/visita/frmVisitasImportar.aspx';
SELECT MOD_ID INTO @mod_carga_nuevo FROM tbl_modulo WHERE MOD_URL = '/aplicacion/visita/frmVisitasImportarNuevo.aspx';
SELECT MOD_ID INTO @mod_carga_detalle FROM tbl_modulo WHERE MOD_URL = '/aplicacion/visita/frmVisitasImportarDetalle.aspx';

SELECT @mod_carga AS listado, @mod_carga_nuevo AS nuevo, @mod_carga_detalle AS detalle;

SET @retorno = NULL;
CALL SP_INS_MODULO_NAVEGACION(@mod_carga, 'NUEVO', @mod_carga_nuevo, 0, @usu_rut, @retorno);
SELECT 'NUEVO' AS accion, @retorno AS retorno;

SET @retorno = NULL;
CALL SP_INS_MODULO_NAVEGACION(@mod_carga, 'DETALLE', @mod_carga_detalle, 0, @usu_rut, @retorno);
SELECT 'DETALLE' AS accion, @retorno AS retorno;

SET @retorno = NULL;
CALL SP_INS_MODULO_NAVEGACION(@mod_carga_nuevo, 'VOLVER', @mod_carga, 0, @usu_rut, @retorno);
SELECT 'VOLVER (nuevo)' AS accion, @retorno AS retorno;

SET @retorno = NULL;
CALL SP_INS_MODULO_NAVEGACION(@mod_carga_detalle, 'VOLVER', @mod_carga, 0, @usu_rut, @retorno);
SELECT 'VOLVER (detalle)' AS accion, @retorno AS retorno;

SELECT MN.MOD_ID_ORIGEN, MO.MOD_NOMBRE AS ORIGEN, MN.MN_ACCION, MN.MOD_ID_DESTINO, MD.MOD_NOMBRE AS DESTINO
FROM tbl_modulo_navegacion MN
INNER JOIN tbl_modulo MO ON MO.MOD_ID = MN.MOD_ID_ORIGEN
INNER JOIN tbl_modulo MD ON MD.MOD_ID = MN.MOD_ID_DESTINO
WHERE MN.MOD_ID_ORIGEN IN (@mod_carga, @mod_carga_nuevo, @mod_carga_detalle);

/* ======================================================================
   BLOQUE 5 - Renombrar MOD_ID 6 a "VISITAS" y mover REGISTRO VISITAS
   (MOD_ID 8) al orden 3. Usa SP_UPD_MODULO con control de concurrencia,
   igual que 20260914_AlinearMenuModulos.sql. Ejecutar cada parte por
   separado y comprobar que RETORNO sea 0.
   ====================================================================== */

/* MOD_ID 6 - CARGA VISITAS -> VISITAS */
SET @usu_rut = '1-9';
SET @mod_id = 6;

SELECT CAT_ID, MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO, MOD_ADMINISTRADOR,
       MOD_MENU_VISIBLE, MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_orden, @url_real, @url_publica, @mod_estado, @administrador,
     @menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, 'VISITAS', 'Visitas', 'Listado de visitas',
    @mod_orden, @url_real, @url_publica, @mod_estado, @administrador,
    @menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 8 - REGISTRO VISITAS: orden 2 -> 3 */
SET @usu_rut = '1-9';
SET @mod_id = 8;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO, MOD_URL, MOD_URL_PUBLICA,
       MOD_ESTADO, MOD_ADMINISTRADOR, MOD_MENU_VISIBLE, MOD_BITACORA_ESTADO, MOD_BITACORA_SP,
       MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido, @url_real, @url_publica,
     @mod_estado, @administrador, @menu_visible, @bitacora_estado, @bitacora_sp,
     @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    3, @url_real, @url_publica, @mod_estado, @administrador,
    @menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* Comprobación final del menú VISITAS. */
SELECT MOD_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_ORDEN, MOD_URL, MOD_MENU_VISIBLE
FROM tbl_modulo
WHERE CAT_ID = 2
ORDER BY MOD_MENU_VISIBLE, MOD_ORDEN, MOD_ID;

/*
    DESPUÉS DE EJECUTAR:
    - Asignar permisos del módulo "CARGAR VISITAS" a los perfiles que
      corresponda desde la web: Módulos x perfil (ver / nuevo).
      El super administrador no requiere asignación.
    - SP_DEL_EMPRESA_VISITA deja de usarse en la web. No se elimina en
      este script: retirarlo por separado cuando la carga esté validada.
*/
