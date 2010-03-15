<?php

require_once('lib/functions.php');
require_once('lib/db.php');
require_once('lib/user.php');

$from = get($_POST, 'from');
if ($from == 'site')
{
    $user = User::load();
}
else
{
    $name = get($_POST, 'user');
    $password = get($_POST, 'hash');
    if (!$name || !$password)
        $user = User::createAnonymous();
    else
        $user = User::createFromLogin($name, $password);
}

if ($user->banned)
{
    if ($from == 'site')
    {
        redirect('index.php');
        die();
    }
    else
    {
        die('User is banned.');
    }
}
else
{
    $src_id = get($_POST, 'src_id');
    if (!$src_id)
    {
        $src = get($_POST, 'src');
        if (!$src)
            die('Source is not provided.');
        if (strlen($src) > 1000)
            die('Source is too long!');
        $exist_id = queryGetString("select id from $table[source] where source = ?", $src);
        if ($exist_id)
        {
            if ($from == 'site')
            { 
                redirect("index.php?p=view&id=$exist_id");
                die();
            }
            else
            {
                die('Source already exists.');
            }            
        }
        query("insert into $table[source] (id, source) values(0, ?)", $src);
        $src_id = mysql_insert_id();
        if (!$src_id)
            die('Cannot insert new source text!');
    }
    $revert_to = (int)get($_POST, 'revert_to');
    if (!$revert_to)
    {
        $trans = get($_POST, 'trans');
        if (!$trans)
            die('Translation is not provided.');
        if (strlen($trans) > 1000)
            die('Translation is too long!');
    }
    else
    {
        if ($revert_to == -1)
            $trans = '';
        else
        {
            $q = query("select translation from $table[translation] where source_id = ? and revision = ?", $src_id, $revert_to);
            $row = $q->fetch();
            if (!$row)
                die('Invalid revision number');
            else
                $trans = $row[0];
            if ($trans == '' || $trans == null)
                $revert_to = -1;
        } 
    }
    $comment = get($_POST, 'comment');
    if (strlen($comment) > 1000)
        die('Comment is too long!');
    query("insert into $table[translation] (id, revision, revert_to, source_id, translation, comment, user_id, user_ip, date)
        select 0, if(max(revision) is null, 0, max(revision)) + 1, ?, ?, ?, ?, ?, ?, now() from $table[translation] where source_id = ?",
        $revert_to, $src_id, $trans, $comment, $user->id, $user->ip, $src_id);
    $trans_id = mysql_insert_id();
    if ($trans_id)
    {
        if ($from == 'site')
            redirect("index.php?p=view&id=$src_id");
        else
            die("OK,$src_id");
    }
    else
        die("Cannot insert new translation!");       
}    

?>
