/*
    dpt_cedula_2: retira SP_UPD_REGLA_ITEM.

    El procedimiento no lo usa la web (se eliminaron ReglaNegocio.ActualizarItem
    y ReglaDatos.UpdateReglaItem), ni la API ni la aplicacion movil.
    Su definicion queda en Scripts/20261001_CorregirComillasBitacoraRegla.sql
    por si fuera necesario recrearlo.

    Ejecutar POR BLOQUES, con respaldo.
*/

USE dpt_cedula_2;

/* BLOQUE 1 - Antes: debe existir una fila. */
SELECT ROUTINE_NAME, DEFINER, LAST_ALTERED
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = 'dpt_cedula_2'
  AND ROUTINE_NAME = 'SP_UPD_REGLA_ITEM';

/* BLOQUE 2 - Retirar el procedimiento. */
DROP PROCEDURE IF EXISTS SP_UPD_REGLA_ITEM;

/* BLOQUE 3 - Despues: no debe devolver filas. */
SELECT ROUTINE_NAME
FROM information_schema.ROUTINES
WHERE ROUTINE_SCHEMA = 'dpt_cedula_2'
  AND ROUTINE_NAME = 'SP_UPD_REGLA_ITEM';
