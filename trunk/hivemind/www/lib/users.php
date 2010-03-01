<?php
    
require_once('lib/db.php');

$doc->title = "Users list";
$doc->content = "<h1>Users list</h1>";
$doc->content .= "<h2>Administrators</h2><ul class='users'>";
$q = query("select id, name from $table[user] where role = 'admin' or role = 'root' order by name");
while ($row = $q->fetch())
{
    $doc->content .= "<li><a href='index.php?p=userinfo&id=$row[0]'>$row[1]</a></li>";
}
$doc->content .= "</ul><h2>Members</h2><ul class='users'>";
$q = query("select id, name from $table[user] where role != 'admin' and role != 'root' order by name");
while ($row = $q->fetch())
{
    $doc->content .= "<li><a href='index.php?p=userinfo&id=$row[0]'>$row[1]</a></li>";
}
$doc->content .= "</ul>";

?>
