<?php

require_once('lib/functions.php');

if (session_start())
{
    $_SESSION['user_id'] = 0;
}

redirect('index.php');

?>