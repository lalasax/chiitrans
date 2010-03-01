<?php

require_once('lib/db.php');
require_once('lib/functions.php');
require_once('lib/user.php');

function invalid()
{
    die("Invalid request.");
}

function error($num)
{
    global $ret;

    redirect("$ret&error=$num");
    die();
}

function revertUserChanges($cond, $comment)
{
    global $table, $user;
    
    query("
        insert into $table[translation] (id, revision, revert_to, source_id, translation, comment, user_id, user_ip, date)
        select 0, if(t1.rev is null, 0, t1.rev) + 1, if(good.rev is null, -1, good.rev), t1.source_id, good.translation, ?, ?, ?, now()  
        from
            (
                select main.source_id, max(main.revision) as rev, max(sub.revision) as maxrev
                from $table[translation] main, $table[translation] sub
                where ($cond) and main.source_id = sub.source_id
                group by main.source_id
                having rev = maxrev 
            ) as t1
        left join
            (
                select source_id, max(revision) as rev, translation
                from $table[translation] main
                where not ($cond)
                group by source_id
            ) as good
            on t1.source_id = good.source_id
    ", $comment, $user->id, $user->ip);
}

function banIp($ip, $ban_date, $ban_reason, $revert, $anon)
{
    global $user, $table;

    if (!$ip)
        error(1);
    query("update $table[ip_data] set ban_date = from_unixtime(?), banned_by = ?, ban_reason = ? where ip = ?", $ban_date, $user->id, $ban_reason, $ip);
    if ($revert)
    {
        if ($anon)   
            revertUserChanges("main.user_id = '0' and main.user_ip = '$ip'", $ban_reason);
        else
            revertUserChanges("main.user_ip = '$ip'", $ban_reason);
    }
}

function processAnonymous($id, $action)
{
    global $user, $table;
    
    if (!$user->isAdmin())
        invalid();
    if ($action == 'admin_comment')
    {
        $comment = get($_POST, 'admin_comment');
        query("update $table[ip_data] set admin_comment = ? where id = ?", $comment, $id);  
    }
    else if ($action == 'ban')
    {
        $ban_date = strtotime(get($_POST, 'ban_date'));
        if (!$ban_date || $ban_date <= time())
            error(3); 
        $revert = get($_POST, 'ban_revert_all') == '1';
        $ban_reason = get($_POST, 'ban_reason');
        $ip = queryGetString("select ip from $table[ip_data] where id = ?", $id);
        banIp($ip, $ban_date, $ban_reason, $revert, true);
    }
    else if ($action == 'unban')
    {
        $unban_reason = get($_POST, 'ban_reason');
        query("update $table[ip_data] set ban_date = from_unixtime(1), banned_by = ?, ban_reason = ? where id = ?", $user->id, $unban_reason, $id);        
    }
    else
    {
        invalid();
    }
}

function processNormal($id, $action)
{
    global $user, $table;
    
    $my = (int)$id == (int)$user->id;
    if (!$my && !$user->isAdmin())
        invalid();
    if ($action == 'comment')
    {
        $comment = get($_POST, 'comment');
        if (strlen($comment) > 10000)
            $comment = substr($comment, 0, 10000);
        query("update $table[user] set comment = ? where id = ?", $comment, $id);
    }
    else if ($action == 'password')
    {
        if (!$my)
            invalid();
        $oldp = md5(get($_POST, 'oldp'));
        $newp = md5(get($_POST, 'newp'));
        $q = query("update $table[user] set password = ? where id = ? and password = ?", $newp, $id, $oldp);
        if (!$q->num_rows)
            error(2); 
    }
    else
    { 
        if (!$user->isAdmin())
            invalid();
        if ($action == 'admin_comment')
        {
            $comment = get($_POST, 'admin_comment');
            query("update $table[user] set admin_comment = ? where id = ?", $comment, $id);  
        }
        else if ($action == 'ban')
        {
            $ban_date = strtotime(get($_POST, 'ban_date'));
            if (!$ban_date || $ban_date <= time())
                error(3); 
            $revert = get($_POST, 'ban_revert_all') == '1';
            $ban_ips = get($_POST, 'ban_ips') == '1';
            $ban_reason = get($_POST, 'ban_reason');
            query("update $table[user] set ban_date = from_unixtime(?), banned_by = ?, ban_reason = ? where id = ?", $ban_date, $user->id, $ban_reason, $id);
            if ($revert && !$ban_ips)
                revertUserChanges("main.user_id = '$id'", $ban_reason);
            if ($ban_ips)
            {
                $q = query("select distinct user_ip from translation where user_id = ?", $id);
                $ips = $q->fetchAll();
                foreach ($ips as $ip)
                {
                    banIp($ip[0], $ban_date, $ban_reason, $revert, false);
                }
            }
        }
        else if ($action == 'unban')
        {
            $unban_reason = get($_POST, 'ban_reason');
            query("update $table[user] set ban_date = from_unixtime(1), banned_by = ?, ban_reason = ? where id = ?", $user->id, $unban_reason, $id);        
        }
        else if ($action == 'promote')
        {
            query("update $table[user] set role = 'admin' where id = ? and id != 1", $id);
        }
        else if ($action == 'demote')
        {
            query("update $table[user] set role = 'user' where id = ? and id != 1", $id);
        }
        else
        {
            invalid();
        }
    }
} 

$user = User::load();
if ($user->banned)
{
    redirect('index.php');
    die();
}
$id = (int)get($_POST, 'id');
$aid = (int)get($_POST, 'aid');
$action = get($_POST, 'action');
if (!$id && !$aid || !$action)
    invalid();
else
{
    if ($aid)
    {
        $ret = "index.php?p=userinfo&aid=$aid";
        processAnonymous($aid, $action);
    }
    else
    {
        $ret = "index.php?p=userinfo&id=$id";
        processNormal($id, $action);
    }
    redirect($ret);
}

?>
