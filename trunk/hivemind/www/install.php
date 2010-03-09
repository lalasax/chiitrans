<?php
require_once('lib/db.php');

if (dbCheck())
    die('Installation is already completed. Please delete file <b>install.php</b> from the server.');

require_once('lib/functions.php');
require_once('lib/template.php');
require_once('lib/user.php');
    
function showForm($name, $error)
{
    global $db_config, $doc;

    $pwd = $db_config['password'] == "" ? "" : "(hidden)";
    $script = <<<EOT
<script>
$(function()
{
    $("#db_password").focus(function()
    {
        this.value = '';
        $(this).unbind('focus');
    });
    $("#install").submit(function(ev)
    {
        if ($("#user").val() == "")
        {
            ev.preventDefault();
            alert("Enter user name!");
            $("#user").focus();
        }
    });
});
</script>
EOT;
    if ($error != "")
        $error = "<p id='error'>$error</p>";
    $doc->content = "
        $script
        <h1>Hivemind installation</h1>
        <form method='post' id='install' action='install.php'>
        <p>Welcome to Hivemind server installation.</p>
        $error
        <p>Please select your name and password. This user will be created with root permissions.</p>
        <div class='fieldset'>
            <label>User name:<br>
            <input type='text' name='user' id='user' value='$name' />
            </label><br>
            <label>Password:<br>
            <input type='password' name='password' />
            </label>
        </div>
        <p>Please check database settings.</p>
        <div class='fieldset'>
            <label>Host:<br>
            <input type='text' name='db_host' value='$db_config[host]' />
            </label><br>
            <label>User:<br>
            <input type='text' name='db_user' value='$db_config[user]' />
            </label><br>
            <label>Password:<br>
            <input type='password' name='db_password' value='$pwd' id='db_password'/>
            </label><br>
            <label>Database name:<br>
            <input type='text' name='db_db' value='$db_config[db]' />
            </label><br>
            <label>Table prefix:<br>
            <input type='text' name='db_prefix' value='$db_config[prefix]' />
            </label>
        </div>
        <input type='submit' value='Proceed' />
        <input type='hidden' name='go' value='1' />
        </form>
    ";
}

$doc = new Document('base');
$doc->title = 'Installation page';

$go = get($_POST, 'go');
if (!$go)
{
    showForm('', '');
}
else
{
    $name = get($_POST, 'user');
    if ($name == "")
    {
        showForm('', 'Enter user name!');
    }
    else
    {
        try
        {
            $db_status = DB_NOT_INITIALIZED;
	    $db_config['host'] = get($_POST, 'db_host');
            $db_config['user'] = get($_POST, 'db_user');
            $pwd = get($_POST, 'db_password'); 
            if ($pwd != '(hidden)')
                $db_config['password'] = $pwd;
            $db_config['db'] = get($_POST, 'db_db');
            $db_config['prefix'] = get($_POST, 'db_prefix');
            if (!dbCreateConnection())
            {
                throw new DBException("Cannot create DB connection.");
            }
            @mysql_query(sprintf("create database %s", mysql_real_escape_string($db_config['db'])));
            dbUpdateTableNames();
            query("drop table if exists $table[user]");
            query(
                "create table $table[user] (
                     id int auto_increment primary key,
                     name varchar(100),
                     password char(32),
                     role varchar(100),
                     comment longtext,
                     admin_comment longtext,
                     ban_date datetime,
                     banned_by int,
                     ban_reason longtext,

                     unique index i_name (name)
                 ) default charset='utf8'");
            query("drop table if exists $table[ip_data]");
            query(
                "create table $table[ip_data] (
                     id int auto_increment primary key,
                     ip varchar(32),
                     admin_comment longtext,
                     ban_date datetime,
                     banned_by int,
                     ban_reason longtext,

                     unique index i_ip (ip)
                 ) default charset='utf8'");
            query("drop table if exists $table[source]");
            query(
                "create table $table[source] (
                     id int auto_increment primary key,
                     source longtext,

                     index i_text (source(256))
                 ) default charset='utf8'");
            query("drop table if exists $table[translation]");
            query(
                "create table $table[translation] (
                     id int auto_increment primary key,
                     revision int,
                     revert_to int,
                     source_id int,
                     translation longtext,
                     comment longtext,
                     user_id int,
                     user_ip varchar(32),
                     date datetime,
                     
                     index i_source_id (source_id),
                     index i_user_id (user_id),
                     index i_user_ip (user_ip),
                     unique index i_source_id_revision (source_id, revision)
                 ) default charset='utf8'");
            $pwd = get($_POST, 'password');
            $pwd = md5($pwd);
            query("insert into $table[user] (id, name, password, role, comment, admin_comment, ban_date, banned_by, ban_reason)
                values(1, ?, ?, 'root', null, null, from_unixtime(0), null, null)",
                $name, $pwd);
            $user = User::load(mysql_insert_id());
            dbSaveConfig();
            $doc->content = "
                <h1>Installation complete</h1>
                <p>Installation completed successfully. Please delete file <b>install.php</b> from the server.</p>
                <p><a href='index.php'>Go to main page</a></p>
            ";
        }
        catch (DBException $ex)
        {
            showForm($name, "Database error: ".$ex->getMessage());
        }
    }
}

echo $doc->render();
?>
