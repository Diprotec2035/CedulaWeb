/*
    Alinea la configuracion de menu de Cédula con el patron de Inventario.
    Ejecutar cada bloque por separado y comprobar que RETORNO sea 0.

    No ejecutar el bloque del MOD_ID 1: ya fue actualizado.
    No usa UPDATE directo: cada cambio pasa por SP_UPD_MODULO y deja bitacora.
*/

USE dpt_cedula_2;

/* Reemplazar solo si el responsable real del cambio es otro usuario. */
SET @usu_rut = '1-9';
SELECT @usu_rut AS rut_bitacora;

/* MOD_ID 2 - PERFILES */
SET @usu_rut = '1-9';
SET @mod_id = 2;
SET @nuevo_administrador = 1;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 3 - USUARIOS */
SET @usu_rut = '1-9';
SET @mod_id = 3;
SET @nuevo_administrador = 1;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 4 - REGLAS */
SET @usu_rut = '1-9';
SET @mod_id = 4;
SET @nuevo_administrador = 1;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 5 - DISPOSITIVOS */
SET @usu_rut = '1-9';
SET @mod_id = 5;
SET @nuevo_administrador = 1;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 6 - CARGA VISITAS */
SET @usu_rut = '1-9';
SET @mod_id = 6;
SET @nuevo_administrador = 0;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 7 - MODULOS X PERFIL */
SET @usu_rut = '1-9';
SET @mod_id = 7;
SET @nuevo_administrador = 1;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 8 - REGISTRO VISITAS */
SET @usu_rut = '1-9';
SET @mod_id = 8;
SET @nuevo_administrador = 0;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 9 - LOG ACCESO WEB */
SET @usu_rut = '1-9';
SET @mod_id = 9;
SET @nuevo_administrador = 0;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 10 - CONEXION DISPOSITIVOS */
SET @usu_rut = '1-9';
SET @mod_id = 10;
SET @nuevo_administrador = 0;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 11 - MIS DATOS */
SET @usu_rut = '1-9';
SET @mod_id = 11;
SET @nuevo_administrador = 0;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 18 - VERSIONES APLICACION */
SET @usu_rut = '1-9';
SET @mod_id = 18;
SET @nuevo_administrador = 1;
SET @nuevo_menu_visible = 0;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 19 - USUARIOS EDITAR */
SET @usu_rut = '1-9';
SET @mod_id = 19;
SET @nuevo_administrador = 0;
SET @nuevo_menu_visible = 1;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 20 - USUARIOS NUEVO */
SET @usu_rut = '1-9';
SET @mod_id = 20;
SET @nuevo_administrador = 0;
SET @nuevo_menu_visible = 1;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 21 - VISITAS EDITAR */
SET @usu_rut = '1-9';
SET @mod_id = 21;
SET @nuevo_administrador = 0;
SET @nuevo_menu_visible = 1;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* MOD_ID 22 - VISITAS NUEVO */
SET @usu_rut = '1-9';
SET @mod_id = 22;
SET @nuevo_administrador = 0;
SET @nuevo_menu_visible = 1;

SELECT CAT_ID, MOD_NOMBRE, MOD_TITULO_PAGINA, MOD_TITULO_CONTENIDO,
       MOD_ORDEN, MOD_URL, MOD_URL_PUBLICA, MOD_ESTADO,
       MOD_BITACORA_ESTADO, MOD_BITACORA_SP, MOD_BITACORA_PARAMETRO, MOD_UPDATED_AT
INTO @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
     @mod_orden, @url_real, @url_publica, @mod_estado,
     @bitacora_estado, @bitacora_sp, @bitacora_parametro, @updated_at_original
FROM tbl_modulo
WHERE MOD_ID = @mod_id;

SET @retorno = NULL;
CALL SP_UPD_MODULO(@mod_id, @cat_id, @mod_nombre, @titulo_pagina, @titulo_contenido,
    @mod_orden, @url_real, @url_publica, @mod_estado, @nuevo_administrador,
    @nuevo_menu_visible, @bitacora_estado, @bitacora_sp, @bitacora_parametro,
    @updated_at_original, @usu_rut, @retorno);
SELECT @mod_id AS modulo, @retorno AS retorno;

/* Validaciones de solo lectura: ejecutar solo despues de obtener retorno 0 en todos los bloques. */
SELECT MOD_ID, MOD_NOMBRE, MOD_MENU_VISIBLE, MOD_ADMINISTRADOR, MOD_ESTADO
FROM tbl_modulo
WHERE MOD_ID IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 18, 19, 20, 21, 22)
ORDER BY MOD_ID;

CALL SP_SEL_MODULO_MENU();
CALL SP_SEL_MODULOS_PERFIL('76001910-0', 2, 1);
