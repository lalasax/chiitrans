<?php

require_once('lib/db.php');

if (!dbCheck())
{
    die('Database failure or not installed. Please reinstall.');
}
else
{
    require_once('lib/user.php');
    $user = User::load();
    if ($user->banned)
    {
        require_once('lib/banned.php');
    }
    else
    {
        require_once('lib/main.php');
    }
}

?>