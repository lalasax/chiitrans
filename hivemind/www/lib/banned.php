<?php

require_once('lib/db.php');
require_once('lib/template.php');

$doc = new Document('base');
$doc->title = "Banned";
$ban_doc = new Document('banned');
if ($user->banned == User::BANNED_BY_USER_ID)
{
    $ban = queryGetRow("select u1.ban_date as ban_date, u2.name as banned_by, u1.ban_reason as ban_reason
                        from $table[user] u1, $table[user] u2
                        where u1.id = ? and u2.id = u1.banned_by", $user->id); 
    $ban_doc->set('ban', "User <b>$user->name</b> is banned until <b>$ban[ban_date]</b> by <b>$ban[banned_by]</b>.");
    $ban_doc->set('logout', "<a href='logout.php'><button>Log out</button></a>");
}
else
{
    $ban = queryGetRow("select t1.ban_date as ban_date, u2.name as banned_by, t1.ban_reason as ban_reason
                        from $table[ip_data] t1, $table[user] u2
                        where t1.ip = ? and u2.id = t1.banned_by", $user->ip); 
    $ban_doc->set('ban', "Your ip address <b>$user->ip</b> is banned until <b>$ban[ban_date]</b> by <b>$ban[banned_by]</b>.");
} 
$ban_doc->set('reason', $ban['ban_reason']);
    
$doc->content = $ban_doc->render();
echo $doc->render();

?>
