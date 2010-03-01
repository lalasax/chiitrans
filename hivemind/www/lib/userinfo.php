<?php

function displayAnonymous($doc, $id)
{
    global $user, $table, $errmsg;
    
    $res = "
        <h2>Anonymous user ($id)</h2>
        <div id='error'>$errmsg</div>
    ";
    if ($user->isAdmin())
    {
        $row = queryGetRow("
            select t1.ip as ip, t1.admin_comment as admin_comment, unix_timestamp(t1.ban_date) as ban_date, t1.ban_reason as ban_reason, t2.name as banned_by
            from $table[ip_data] t1 left join $table[user] t2 on t1.banned_by = t2.id
            where t1.id = ?", $id);
        if (!$row)
            displayNotFound($doc);
        else
        {
            $now = time();
            $ban_date = (int)$row['ban_date'];
            $banned = false;
            if ($ban_date == 1)
            {
                $reason = e($row['ban_reason']);
                $ban = "<p>Ban relieved by <b>$row[banned_by]</b>. Reason: <b>$reason</b></p>";
            }
            else if ($now <= $ban_date)
            {
                $reason = e($row['ban_reason']);
                $date = formatDate($ban_date);
                $ban = "<p>User is banned until <b>$date</b> by <b>$row[banned_by]</b>. Reason: <b>$reason</b></p>";
                $banned = true;
            }
            if ($banned)
            {
                $ban_form = "
                    <button id='banform_btn'>Remove ban</button>
                    <form method='post' id='banform' class='hidden' action='user.php'>
                    <div class='fields'>
                    <label>Why do you want to remove ban?<br>
                    <textarea name='ban_reason'></textarea>
                    </label>
                    </div>
                    <input type='submit' value='Remove ban' />
                    <input type='hidden' name='action' value='unban' />
                    <input type='hidden' name='aid' value='$id' />
                    </form>
                ";                 
            }
            else
            {
                $ban_until = formatDate($now);
                $ban_form = "
                    <button id='banform_btn'>Ban user</button>
                    <form method='post' id='banform' class='hidden' action='user.php'>
                    <div class='fields'>
                    <label>Reason for ban:<br>
                    <textarea name='ban_reason'></textarea>
                    </label><br>
                    <label>Ban until:<br>
                    <input type='text' id='ban_date' name='ban_date' value='$ban_until' />
                    </label><br>
                    <label>
                    <input type='checkbox' class='nowidth' name='ban_revert_all' value='1' />
                    Revert all changes made by this user
                    </label><br>
                    <label>
                    </div>
                    <input type='submit' value='Ban user' />
                    <input type='hidden' name='action' value='ban' />
                    <input type='hidden' name='aid' value='$id' />
                    </form>
                ";                 
            }
            $admin_comment = e($row['admin_comment']);
            
            $res .= "
                <p>IP: $row[ip]</p>
                $ban
                <form method='post' action='user.php'>
                <div class='fields'>
                <label>Administrator comment (visible to administrators only):<br>
                <textarea name='admin_comment'>$admin_comment</textarea>
                </label>
                </div>
                <input type='submit' value='Update administrator comment' />
                <input type='hidden' name='action' value='admin_comment' />
                <input type='hidden' name='aid' value='$id' />
                </form>
                $ban_form
                <script>
                    $(function()
                    {
                        $('#banform_btn').click(function()
                        {
                            $('#banform').toggle();
                        });
                    });
                </script>
            "; 
        }
    }
    $res .= "<p><a href='index.php?aby=$id'>View user submissions</a></p>";

    $doc->content = $res;
}

function displayNormal($doc, $id)
{
    global $user, $table, $errmsg;
    
    if (!$user->isAdmin())
    {
        $row = queryGetRow("
            select t1.name as name, t1.role as role, t1.comment as comment
            from $table[user] t1
            where t1.id = ?", $id);
    }
    else
    {
        $row = queryGetRow("
            select t1.name as name, t1.role as role, t1.comment as comment, t1.admin_comment as admin_comment, unix_timestamp(t1.ban_date) as ban_date, t1.ban_reason as ban_reason, t2.name as banned_by
            from $table[user] t1 left join $table[user] t2 on t1.banned_by = t2.id
            where t1.id = ?", $id);
    }
    if (!$row)
    {
        displayNotFound($doc);
        return;
    }
    $name = $row['name'];
    if ($row['comment'])
    {
        $comment = "<div class='user_comment'>".f($row['comment'])."</div>";
        $_comment = e($row['comment']);
    }
    $my = (int)$user->id == (int)$id;
    if ($my)
    {
        $user_form = "
            <form method='post' action='user.php'>
                <div class='fields'>
                <label>Write a few things about yourself (optional):<br>
                <textarea name='comment'>$_comment</textarea>
                </label>
                </div>
                <input type='submit' value='Update information' />
                <input type='hidden' name='action' value='comment' />
                <input type='hidden' name='id' value='$id' />
            </form>
            <button id='passform_btn'>Change password</button>
            <form method='post' action='user.php' id='passform' class='hidden'>
                <div class='fields'>
                <label>Old password:<br>
                <input type='password' name='oldp' />
                </label><br>
                <label>New password:<br>
                <input type='password' name='newp' id='newp' />
                </label><br>
                <label>Repeat new password:<br>
                <input type='password' id='newp2' />
                </label>
                </div>
                <input type='submit' value='Change password' />
                <input type='hidden' name='action' value='password' />
                <input type='hidden' name='id' value='$id' />
            </form>
        ";
    }
    else if ($user->isAdmin())
    {
        $user_form = "
            <form method='post' action='user.php'>
                <div class='fields'>
                <label>User information:<br>
                <textarea name='comment'>$_comment</textarea>
                </label>
                </div>
                <input type='submit' value='Update information' />
                <input type='hidden' name='action' value='comment' />
                <input type='hidden' name='id' value='$id' />
            </form>
        ";
    }
    if ($user_form)
    {
        $user_form .= "
            <script>
                $(function()
                {
                    $('#passform_btn').click(function()
                    {
                        $('#passform').toggle();
                    });
                    $('#passform').submit(function(ev)
                    {
                        if ($('#newp').val() != $('#newp2').val())
                        {
                            $('#error').html('Error: passwords do not match');
                            ev.preventDefault();
                        }
                    });
                });
            </script>
        ";
    }  
    
    if ($row['role'] == 'root')
        $role = 'Owner';
    else if ($row['role'] == 'admin')
        $role = 'Administrator';
    else if ($row['role'] == 'user')
        $role = 'Member';
    
    $res = "
        <h2>$name</h2>
        <div id='error'>$errmsg</div>
        <p>$role</p>
        $comment
        $user_form
        ";
    if ($user->isAdmin())
    {
        $now = time();
        $ban_date = (int)$row['ban_date'];
        $banned = false;
        if ($ban_date == 1)
        {
            $reason = e($row['ban_reason']);
            $ban = "<p>Ban relieved by <b>$row[banned_by]</b>. Reason: <b>$reason</b></p>";
        }
        else if ($now <= $ban_date)
        {
            $reason = e($row['ban_reason']);
            $date = formatDate($ban_date);
            $ban = "<p>User is banned until <b>$date</b> by <b>$row[banned_by]</b>. Reason: <b>$reason</b></p>";
            $banned = true;
        }
        if ($banned)
        {
            $ban_form = "
                <button id='banform_btn'>Remove ban</button>
                <form method='post' id='banform' class='hidden' action='user.php'>
                <div class='fields'>
                <label>Why do you want to remove ban?<br>
                <textarea name='ban_reason'></textarea>
                </label>
                </div>
                <input type='submit' value='Remove ban' />
                <input type='hidden' name='action' value='unban' />
                <input type='hidden' name='id' value='$id' />
                </form>
            ";                 
        }
        else
        {
            $ban_until = formatDate($now);
            $ban_form = "
                <button id='banform_btn'>Ban user</button>
                <form method='post' id='banform' class='hidden' action='user.php'>
                <div class='fields'>
                <label>Reason for ban:<br>
                <textarea name='ban_reason'></textarea>
                </label><br>
                <label>Ban until:<br>
                <input type='text' id='ban_date' name='ban_date' value='$ban_until' />
                </label><br>
                <label>
                <input type='checkbox' class='nowidth' name='ban_ips' value='1' />
                Also ban all IPs of this user
                </label><br>
                <label>
                <input type='checkbox' class='nowidth' name='ban_revert_all' value='1' />
                Revert all changes made by this user
                </label><br>
                <label>
                </div>
                <input type='submit' value='Ban user' />
                <input type='hidden' name='action' value='ban' />
                <input type='hidden' name='id' value='$id' />
                </form>
            ";                 
        }
        $admin_comment = e($row['admin_comment']);
        
        $ips = array();
        $q = query("select distinct t1.user_ip, t2.id from $table[translation] t1, $table[ip_data] t2 where t1.user_id = ? and t1.user_ip = t2.ip", $id);
        while ($rr = $q->fetch())
        {
            $ips[] = "<a href='index.php?p=userinfo&aid=$rr[1]'>$rr[0]</a>";
        }
        $ips = implode('<br>', $ips);
        
        if ($row['role'] == 'admin')
        {
            $promote_form = "
                <form method='post' action='user.php' class='addmargin' id='promote_form'>
                <input type='submit' value='Demote to member' />
                <div id='promote_msg'></div>
                <input type='hidden' name='action' value='demote' />
                <input type='hidden' name='id' value='$id' />
                </form>
            ";
        }
        else if ($row['role'] != 'root')
        {
            $promote_form = "
                <form method='post' action='user.php' class='addmargin' id='promote_form'>
                <input type='submit' value='Promote to administrator' />
                <div id='promote_msg'></div>
                <input type='hidden' name='action' value='promote' />
                <input type='hidden' name='id' value='$id' />
                </form>
            ";
        }        
        
        $res .= "
            <p>IPs used by this user:</p>
            <p>$ips</p>
            $ban
            <form method='post' action='user.php'>
            <div class='fields'>
            <label>Administrator comment (visible to administrators only):<br>
            <textarea name='admin_comment'>$admin_comment</textarea>
            </label>
            </div>
            <input type='submit' value='Update administrator comment' />
            <input type='hidden' name='action' value='admin_comment' />
            <input type='hidden' name='id' value='$id' />
            </form>
            $ban_form
            $promote_form
            <script>
                var promote_confirmed = false;
                $(function()
                {
                    $('#banform_btn').click(function()
                    {
                        $('#banform').toggle();
                    });
                    $('#promote_form').submit(function(ev)
                    {
                        if (!promote_confirmed)
                        {
                            promote_confirmed = true;
                            $('#promote_msg').html('Are you sure? Click again to confirm.');
                            ev.preventDefault();
                        }
                    });
                });
            </script>
        "; 
    }
    $res .= "<p><a href='index.php?by=$id'>View user submissions</a></p>";
        
    $doc->content = $res;
}

function displayNotFound($doc)
{
    $doc->content = "<p id='error'>User not found!</p>";
}

$doc->title = "User information"; 

$id = (int)get($_GET, 'id');
$aid = (int)get($_GET, 'aid');
if (!$id && !$aid)
    displayNotFound($doc);
else
{
    $error = get($_GET, 'error');
    if ($error == '1')
    {
        $errmsg = 'User not found!';
    }
    else if ($error == '2')
    {
        $errmsg = 'Failed to change password: old password is incorrect!';  
    }
    else
    {
        if ($user->isAdmin())
        {
            if ($error == '3')
            {
                $errmsg = 'Failed to ban user: ban date is invalid!';
            }
        }
    }

    if ($aid)
    {
        displayAnonymous($doc, $aid);
    }
    else
    {
        displayNormal($doc, $id);
    }
}

?>
