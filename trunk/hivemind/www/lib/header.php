<?php

if (!$user->id)
{
    $loginform = "
        Login:
        <form id='toplogin' method='post' action='login.php'>
        <input type='text' name='user' />
        <input type='password' name='password' />
        <input type='submit' style='display:none' />
        </form>
        or <a href='index.php?p=register'>register</a>&nbsp;&nbsp;
    ";
}
else
{
    $loginform = "
        <a href='logout.php'><button>Log out</button></a>
    "; 
} 

function genMenuItem($caption, $page)
{
    global $p;
    
    if ($p == $page)
        $cl = " class='active'";
    else
        $cl = '';
    if ($page == 'default')
        $pp = '';
    else
        $pp = "?p=$page";
    return "<li><a$cl href='index.php$pp'>$caption</a></li>";
}

$menu = "";
$menu .= genMenuItem('View last submissions', 'default');
if ($user->isAdmin())
{ 
    $menu .= genMenuItem('View users', 'users');
}

$name = $user->namelink();
$header = "
    <table id='topbar'><tr>
        <td class='topleft'>Welcome, $name.</td>
        <td class='topright'>$loginform</td>
    </tr></table>
    <ul class='menu'>$menu</ul>
";

$doc->set('header', $header);

?>
