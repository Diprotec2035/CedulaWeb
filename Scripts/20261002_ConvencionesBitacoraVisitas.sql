/*
    dpt_cedula_2: alinea la bitacora de visitas con las convenciones de
    Inventario y quita ON UPDATE CURRENT_TIMESTAMP de EV_UPDATED_AT.

    - Indice IX_EMPRESA_VISITA_BITACORA_USUARIO -> ..._USU_RUT, como todas las bitacoras.
    - Llaves foraneas con el formato FK_TABLA:REFERENCIA.
    - AUTO_INCREMENT desde 100000, como las demas bitacoras. Los ID existentes no cambian.
    - EV_UPDATED_AT sin ON UPDATE: SP_UPD_EMPRESA_VISITA_V2 ya lo asigna
      explicitamente. El unico que dependia del ON UPDATE era SP_UPD_EMPRESA_VISITA,
      que se retira en 20261002_DeshabilitarProcedimientosSinUso.sql.
    - Comentario de bitacora "Visitante creado/modificado" -> "Visita creada/modificada"
      en los registros existentes. Los nuevos ya lo usan despues de
      20261002_PropietarioProcedimientosVisitasReglas.sql.

    Ejecutar DESPUES de los otros tres scripts del 20261002, POR BLOQUES y con respaldo.
*/

USE dpt_cedula_2;

/* BLOQUE 1 - Diagnostico. */
SHOW CREATE TABLE `tbl_empresa_visita_bitacora`;
SHOW CREATE TABLE `tbl_empresa_visita`;

SELECT EVB_COMENTARIOS, COUNT(*) AS filas
FROM tbl_empresa_visita_bitacora
GROUP BY EVB_COMENTARIOS;

/* BLOQUE 2 - Indice y llaves foraneas de la bitacora. */
ALTER TABLE `tbl_empresa_visita_bitacora` RENAME INDEX `IX_EMPRESA_VISITA_BITACORA_USUARIO` TO `IX_EMPRESA_VISITA_BITACORA_USU_RUT`;

ALTER TABLE `tbl_empresa_visita_bitacora` DROP FOREIGN KEY `FK_EMPRESA_VISITA_BITACORA_USUARIO`;
ALTER TABLE `tbl_empresa_visita_bitacora` ADD CONSTRAINT `FK_EMPRESA_VISITA_BITACORA:USUARIO` FOREIGN KEY (`USU_RUT`) REFERENCES `tbl_usuario` (`USU_RUT`);

ALTER TABLE `tbl_empresa_visita_bitacora` DROP FOREIGN KEY `FK_EMPRESA_VISITA_BITACORA_VISITA`;
ALTER TABLE `tbl_empresa_visita_bitacora` ADD CONSTRAINT `FK_EMPRESA_VISITA_BITACORA:EMPRESA_VISITA` FOREIGN KEY (`EMP_RUT`, `EV_RUT`) REFERENCES `tbl_empresa_visita` (`EMP_RUT`, `EV_RUT`);

ALTER TABLE `tbl_empresa_visita_bitacora` AUTO_INCREMENT = 100000;

/* BLOQUE 3 - EV_UPDATED_AT sin ON UPDATE. */
ALTER TABLE `tbl_empresa_visita`
    MODIFY `EV_UPDATED_AT` DATETIME(6) NOT NULL DEFAULT (CURRENT_TIMESTAMP(6));

/* BLOQUE 4 - Comentarios existentes de la bitacora. */
UPDATE tbl_empresa_visita_bitacora SET EVB_COMENTARIOS = 'Visita creada' WHERE EVB_COMENTARIOS = 'Visitante creado';
UPDATE tbl_empresa_visita_bitacora SET EVB_COMENTARIOS = 'Visita modificada' WHERE EVB_COMENTARIOS = 'Visitante modificado';

/* BLOQUE 5 - Verificacion: indice ..._USU_RUT, llaves FK_...:..., EV_UPDATED_AT sin
   ON UPDATE y ningun comentario "Visitante ...". */
SHOW CREATE TABLE `tbl_empresa_visita_bitacora`;
SHOW CREATE TABLE `tbl_empresa_visita`;

SELECT EVB_COMENTARIOS, COUNT(*) AS filas
FROM tbl_empresa_visita_bitacora
GROUP BY EVB_COMENTARIOS;
