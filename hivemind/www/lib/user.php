<?php

require_once('lib/db.php');
require_once('lib/functions.php');

class User
{
    const NOT_BANNED = 0;
    const BANNED_BY_USER_ID = 1;
    const BANNED_BY_IP = 2;
    
    public $id;
    public $aid;
    public $ip;
    public $name;
    public $banned;
    public $role;
    
    private function __construct()
    {
        $this->ip = $_SERVER['REMOTE_ADDR'];
        $this->aid = 0;
    }
    
    private function setData($data)
    {
        $this->id = (int)$data['id'];
        $this->name = $data['name'];
        if ($this->id == 1)
            $this->role = 'root';
        else
            $this->role = $data['role'];
        $now = time();
        $this->banned = self::NOT_BANNED;
        if ($this->role != 'root')
        {
            if ((int)$data['user_id_ban_date'] >= $now)
            {
                $this->banned = self::BANNED_BY_USER_ID;
            }
            else if ((int)$data['ip_ban_date'] >= $now)
            {
                $this->banned = self::BANNED_BY_IP;
            }
        }
    }
    
    public function isAdmin()
    {
        return $this->role == 'root' || $this->role == 'admin';
    }
    
    public function createIpRecord()
    {
        global $table;

        try
        {
            query("insert into $table[ip_data] (id, ip, admin_comment, ban_date, banned_by, ban_reason) values(0, ?, null, from_unixtime(0), null, null)", $this->ip);
            return mysql_insert_id();
        }
        catch (DBException $ex)
        {
            return 0;
        } 
    }
    
    public function namelink()
    {
        return constructName($this->id, $this->id ? $this->name : $this->aid); 
    }
    
    private static function createFromData($user, $data)
    {
        if (!$data || !$data['id'])
        {
            return self::createAnonymous();
        }
        else
        {
            if ($data['ip_ban_date'] === null)
            {
                $user->createIpRecord();
                $data['ip_ban_date'] = 0;
            }
            else
            {
                $data['ip_ban_date'] = strtotime($data['ip_ban_date']);
            }
            if ($data['user_id_ban_date'] === null)
            {
                $data['user_id_ban_date'] = 0;
            }
            else
            {
                $data['user_id_ban_date'] = strtotime($data['user_id_ban_date']);
            }
            $user->setData($data);
            return $user;
        }
    }
    
    public static function createFromId($id)
    {
        global $table;
        
        $user = new self();
        $data = queryGetRow(
            "select $table[user].id as id, $table[user].name as name, $table[user].role as role, $table[user].ban_date as user_id_ban_date, $table[ip_data].ban_date as ip_ban_date
             from $table[user] left join $table[ip_data] on $table[ip_data].ip = ?
             where $table[user].id = ?",
            $user->ip, $id);
        return self::createFromData($user, $data);
    }
    
    public static function createFromLogin($name, $pass)
    {
        global $table;
        
        $user = new self();
        $data = queryGetRow(
            "select $table[user].id as id, $table[user].name as name, $table[user].role as role, $table[user].ban_date as user_id_ban_date, $table[ip_data].ban_date as ip_ban_date
             from $table[user] left join $table[ip_data] on $table[ip_data].ip = ?
             where $table[user].name = ? and $table[user].password = ?",
            $user->ip, $name, $pass);
        return self::createFromData($user, $data);
    }
    
    public static function createAnonymous()
    {
        global $table;
        
        $user = new self();
        $res = queryGetRow("select id, unix_timestamp(ban_date) as ban_date from $table[ip_data] where ip = ?", $user->ip);
        if (!$res)
        {
            $id = $user->createIpRecord();
            if (!$id)
                throw new Exception('Cannot create anonymous user');
            $ban_date = 0;
        }
        else
        {
            $id = $res['id'];
            $ban_date = $res['ban_date'];
        }
        $data['id'] = 0;
        $user->aid = $id;
        $data['name'] = 'Anonymous'.$id;
        $data['role'] = 'user';
        $data['user_id_ban_date'] = 0;
        $data['ip_ban_date'] = $ban_date;
        $user->setData($data);
        return $user;
    }
    
    public static function load($id = 0)
    {
        if (!@session_start())
            throw new Exception("Cannot start session");
        if ($id)
            $_SESSION['user_id'] = $id;
        if (isset($_SESSION['user_id']))
        {
            return self::createFromId($_SESSION['user_id']);
        }
        else
        {
            return self::createAnonymous();
        }
    }
}

?>
