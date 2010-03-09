<?php

$magic_quotes = get_magic_quotes_gpc();

function get($arr, $name)
{
    global $magic_quotes;
    
    $res = $arr[$name];
    if ($magic_quotes)
        $res = stripslashes($res);
    return $res;
}

function e($s)
{
    return htmlspecialchars($s);
}

function constructName($user_id, $name)
{
    if ($user_id)
        return "<a href='index.php?p=userinfo&id=$user_id'>".e($name)."</a>";
    else
    {
        if ($name)
            return "<a href='index.php?p=userinfo&aid=$name'>Anonymous$name</a>";
        else
            return "Anonymous";
    } 
}

function redirect($where)
{
    header("Location: $where");
}

function f($s)
{
    $s = e($s);
    $s = str_replace("\n", '<br>', $s);
    $s = preg_replace('/(?<! ) /', "\n", $s);
    $s = str_replace(' ', '&nbsp;', $s);
    $s = str_replace("\n", ' ', $s);
    return $s;
}

function formatDate($d)
{
    return date('F d, Y H:i:s', $d);
}

?>
