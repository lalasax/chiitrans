<?php

require_once('lib/functions.php');
require_once('lib/user.php');

function showForm($name = '', $error = '')
{
    $doc = new Document('base');
    $doc->title = 'Login';
    if ($error != '')
        $error = "<p id='error'>$error</p>";
    $doc->content = "
        <h1>Authorization form</h1>
        <form method='post' action='login.php'>
        $error
        <div class='fieldset'>
        <label>User name:<br>
        <input type='text' name='user' value='$name' id='name'/>
        </label><br>
        <label>Password:<br>
        <input type='password' name='password' />
        </label>
        </div>
        <input type='submit' value='Enter' />
        </form>
        <script>\$(function() {\$('#name').focus();});</script>
    ";
    echo $doc->render(); 
}

$name = get($_POST, 'user');
if ($name)
{
    $pass = get($_POST, 'password');
    $pass = md5($pass);
    $u = User::createFromLogin($name, $pass);
    if ($u->id)
    {
        if (session_start())
        {
            $_SESSION['user_id'] = $u->id;
        }
        redirect('index.php');
    }
    else
    {
        require_once('lib/template.php');
        showForm($name, 'Invalid user name or password!');
    }
}
else
{
    require_once('lib/template.php');
    showForm();
} 

?>