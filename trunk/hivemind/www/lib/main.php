<?php

require_once('lib/db.php');
require_once('lib/functions.php');
require_once('lib/template.php');
require_once('lib/user.php');

$doc = new Document('main');

$p = get($_GET, 'p');
$pages = array('default', 'view', 'register', 'userinfo');
if ($user->isAdmin())
{
    $pages = array_merge($pages, array('users'));
} 
if (!$p || array_search($p, $pages) === false)
    $p = 'default';

require_once('lib/header.php');
require_once("lib/$p.php");

echo $doc->render();

?>
