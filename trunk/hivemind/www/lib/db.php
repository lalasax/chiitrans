<?php

require_once('config/db_config.php');

$table_names = array("user", "ip_data", "source", "translation");
$table = array();

function dbUpdateTableNames()
{
    global $db_config, $table_names, $table;
    
    foreach ($table_names as $name)
    {
        $table[$name] = $db_config["prefix"].$name;
    }
}

dbUpdateTableNames();

define('DB_NOT_INITIALIZED', 0);
define('DB_OK', 1);
define('DB_FAIL', 2);

$db_status = DB_NOT_INITIALIZED;
$connection = null; 

class DBException extends Exception
{
    public function __construct($message, $code = 0) {
        parent::__construct($message, $code);
    }
}

class Query
{
    private $resource;
    public $num_rows;
    
    public function __construct($resource)
    {
        if ($resource === TRUE)
        {
            $this->num_rows = mysql_affected_rows();
            $this->resource = null;
        }
        else
        { 
            $this->resource = $resource;
            $this->num_rows = mysql_num_rows($this->resource);
        }    
    }
    
    public function fetch($result_type = MYSQL_BOTH)
    {
        if (!$this->resource)
            return false;
        $res = mysql_fetch_array($this->resource, $result_type);
        if (!$res)
            $this->free();
        return $res;            
    }
    
    public function fetchAll($result_type = MYSQL_BOTH)
    {
        $res = array();
        while ($row = $this->fetch($result_type))
            $res[] = $row;
        return $res;
    }
    
    public function free()
    {
        if ($this->resource)
        {
            mysql_free_result($this->resource);
            $this->resource = null;
        }
    } 
}

function dbCreateConnection()
{
    global $db_config;
    
    $connection = @mysql_connect($db_config['host'], $db_config['user'], $db_config['password']);
    return $connection;
}

function dbSelectDB()
{
    global $db_config;
    
    return @mysql_select_db($db_config['db']);
}

function dbConfigure()
{
    return mysql_query("set names utf8") && @mysql_set_charset('utf8');
}

function dbConnect()
{
    global $db_status;
    
    if ($db_status == DB_NOT_INITIALIZED)
    {
        $db_status = (dbCreateConnection() && dbSelectDB() && dbConfigure()) ? DB_OK : DB_FAIL;
    }
    if ($db_status == DB_FAIL)
        throw new DBException('Database connection failure.');
}

function dbCheck()
{
    global $table;
    
    try
    {
        dbConnect();
        $res = mysql_query("show tables like '$table[user]'");
        return $res && mysql_num_rows($res) >= 1;
    }
    catch (DBException $e)
    {
        return false;
    }
}

function _query($args)
{
    dbConnect();
    $query = $args[0];
    array_shift($args);
    $q = vsprintf(str_replace('?', '\'%s\'', $query), array_map('mysql_escape_string', $args));
    //echo "<pre>".htmlspecialchars($q)."</pre>";
    $res = mysql_query($q);
    if (!$res)
        throw new DBException(mysql_error()."<br> in query: \"".htmlspecialchars($q)."\"");
    return new Query($res);
}

function query()
{
    return _query(func_get_args());
}

function queryGetRow()
{
    $q = _query(func_get_args());
    $res = $q->fetch();
    $q->free();
    return $res;
}

function queryGetString()
{
    $q = _query(func_get_args());
    $res = $q->fetch(MYSQL_NUM);
    $q->free();
    if (is_array($res))
        return $res[0];
    else
        return $res;
}

function dbSaveConfig()
{
    global $db_config;
    
    $config = "<?php\n\n\$db_config = ".var_export($db_config, true)."\n\n?".'>';
    file_put_contents('config/db_config.php', $config);
}

?>