<?php

require_once('lib/db.php');
require_once('lib/functions.php');

$q = $_GET['q'];
if (!$q) die();
try
{
    $qq = queryGetRow("select t.source_id, t.translation from $table[translation] t, $table[source] src where src.source = ? and t.source_id = src.id order by t.id desc limit 1", $q);
    if (!$qq)
        echo '{"success":"1","id":"0","result":""}';
    else 
        echo '{"success":"1","id":"'.$qq[0].'","result":"'.addslashes($qq[1]).'"}'; 
}
catch (Exception $ex)
{
    echo '{"success":"0","error":"'.addslashes($ex->getMessage()).'"}';
}

?>
