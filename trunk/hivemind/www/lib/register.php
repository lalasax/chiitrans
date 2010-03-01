<?php

require_once('lib/functions.php');
require_once('lib/db.php');
require_once('lib/user.php');

function showForm($name = '', $error = '')
{
    global $doc;
    
    $doc->title = 'Registration';
    $doc->content = "
        <h1>Registration form</h1>
        <form method='post' action='index.php?p=register' id='regform'>
        <p id='error'>$error</p>
        <div class='fieldset'>
        <label>User name:<br>
        <input type='text' name='user' value='$name' id='name'/>
        </label><br>
        <label>Password:<br>
        <input type='password' name='password' id='pass1' />
        </label><br>
        <label>Repeat password:<br>
        <input type='password' id='pass2' />
        </label>
        </div>
        <input type='submit' value='Submit registration' />
        </form>
        <script>
        $(function()
        {
            $('#name').focus();
            $('#regform').submit(function(ev)
            {
                if ($('#pass1').val() != $('#pass2').val())
                {
                    $('#error').html('Passwords do not match!');
                    ev.preventDefault();
                }
            });
        });
        </script>
    ";
}

$name = get($_POST, 'user');
if ($name)
{
    if ($name == trim(e($name)))
    {
        try
        {
            $pass = get($_POST, 'password');
            $pass = md5($pass);
            query("
                insert into $table[user] (id, name, password, role, comment, admin_comment, ban_date, banned_by, ban_reason)  
                values (0, ?, ?, 'user', null, null, from_unixtime(0), null, null)", $name, $pass);
            $id = mysql_insert_id();
            $ok = true;    
        }
        catch (DBException $ex)
        {
        }
    }
    if ($ok)
    {
        if (!$id)
            die('Unexpected error.');
        User::load($id);
        redirect('index.php');
        die();
    }
    else
    {
        showForm($name, 'Cannot create user. Please choose another name.');
    }
}
else
{
    showForm();
} 

?>