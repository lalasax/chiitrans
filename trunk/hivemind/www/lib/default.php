<?php

function searchForm()
{
    global $search;
    
    $s = addslashes($search);
    $res = "<form id='search' method='get' action='index.php'>
            <table style='width:100%'><tr>
            <td><input type='text' id='q' name='q' value='$s' /></td>
            <td id='submit'><input type='submit' id='submit' value='Search' /></td>
            </tr></table>
            </form>
            <script>
                $(function()
                {
                    $('form#search').submit(function(ev)
                    {
                        if ($('#q').val() == '')
                            ev.preventDefault();
                    });
                });
            </script>
            ";
    
    return $res; 
}

function submitForm()
{
    global $search;
    
    $s = e($search);
    $res = "<p>Nothing has been found! Submit a new translation?</p>
            <form id='newsubmit' method='post' action='submit.php'>
            <div class='fields'>
            <label>Source text:<br>
            <div class='text'>$s</div>
            </label>
            <div id='error'></div>
            <label>Translation:<br>
            <textarea name='trans' id='trans'></textarea>
            </label><br>
            <label>Comment (optional):<br>
            <textarea name='comment'></textarea>
            </label><br>
            </div>
            <input type='submit' />
            <input type='hidden' name='from' value='site' />
            <input type='hidden' name='src' value='".addslashes($search)."' />
            </form>
            <script>
            $(function()
            {
                $('#trans').focus();
                $('#newsubmit').submit(function(ev)
                {
                    if ($('#trans').val() == '')
                    {
                        $('#error').html('Please fill the translation field.');
                        ev.preventDefault();
                    }
                });
            });
            </script>
        ";
        
    return $res;
}

function showLastSubmits()
{
    global $table, $search;

    $by = (int)get($_GET, 'by');
    $aby = (int)get($_GET, 'aby');
    $res = "";
    if ($search)
        $head = "<h2>Search results</h2>";
    else if ($by || $aby)
        $head = "<h2>User submissions</h2>";
    $res = "";
    $max_id = (int)queryGetString("select max(id) from $table[translation]");
    $skip = (int)get($_GET, 'skip');
    if ($skip < 0)
        $skip = 0;
    $perpage = (int)get($_GET, 'perpage');
    if ($perpage <= 0 || $perpage > 100)
        $perpage = 20;
    $to = $max_id - $skip;
    $from = $to - $perpage * 2;

    $qq = "select t1.id as id, t1.source_id as source_id, t2.source as source, t1.translation as translation, t1.revision as revision, t1.revert_to as revert_to, t1.comment as comment, t1.user_id as user_id, if(t1.user_id = 0, ip.id, u1.name) as name, unix_timestamp(t1.date) as _date
           from $table[translation] t1 left join $table[ip_data] ip on t1.user_ip = ip.ip left join $table[user] u1 on t1.user_id = u1.id inner join $table[source] t2 on t1.source_id = t2.id";

    $num_rows = $max_id;
    
    if ($search)
    {
        $q = query("$qq where t2.source like ? order by t1.id desc limit $perpage", $search);
    }
    else if ($by || $aby)
    {
        if ($aby)
        {
            $num_rows = (int)queryGetString("select count(*) from $table[translation] t1, $table[ip_data] ip where ip.id = ? and t1.user_id = 0 and t1.user_ip = ip.ip", $aby); 
            $q = query("$qq where t1.user_id = 0 and ip.id = ? order by t1.id desc limit $skip, $perpage", $aby);
        }
        else
        {
            $num_rows = (int)queryGetString("select count(*) from $table[translation] where user_id = ?", $by); 
            $q = query("$qq where t1.user_id = ? order by t1.id desc limit $skip, $perpage", $by);
        }
    }
    else
    {
        $q = query("$qq where t1.id > $from and t1.id <= $to order by t1.id desc limit $perpage");
        if ($q->num_rows < $perpage && $to - $perpage >= 0)
        {
            $q->free();
            $q = query("$qq where t1.id > $from order by t1.id desc limit $perpage");
        } 
    }
    
    if (!$search && $num_rows > 0)
    {
        $res .= "<p>Pages: ";
        $pages_num = (int)(($num_rows + $perpage - 1) / $perpage);
        $ct = 0;
        for ($i = 0; $i < $pages_num; ++$i)
        {
            $num = $i + 1;
            if ($skip >= $ct && $skip < $ct + $perpage)
                $num = "<b>$num</b>";
            $link = "<a href='index.php?${qs}skip=$ct&perpage=$perpage'>$num</a>";
            $res .= $link.' ';
            $ct += $perpage;
        }
        $res .= '</p>';
    }

    $res .= "<table id='submissions'>";

    $i = 0;
    while ($row = $q->fetch(MYSQL_ASSOC))
    {
        $comment = "<span class='comment'>".e($row['comment'])."</span>";
        if ($row['revert_to'])
        {
            if ($row['revert_to'] == '-1')
                $comment = "Deleted. $comment";
            else
                $comment = "Reverted to revision $row[revert_to]. $comment";
        }
        $name = constructName($row['user_id'], $row['name']);
        
        $cl = ($i++ % 2) ? 'c2' : 'c1';
        $res .= "<tr class='$cl'>";
        $res .= "<td><a href='index.php?p=view&id=$row[source_id]'>".e($row['source'])."</a></td>";
        $res .= "<td>".e($row['translation'])."</td>";
        $res .= "<td>$comment</td>";
        $res .= "<td>$name</td>";
        $res .= "<td>$row[revision]</td>";
        $res .= "<td>".formatDate($row['_date'])."</td>";
        $res .= "</tr>";
    }
    $res .= "</table>";
    
    if (!$search)
    {
        if ($num_rows > 0)
        {
            $res .= "<p>Pages: ";
            $pages_num = (int)(($num_rows + $perpage - 1) / $perpage);
            $ct = 0;
            for ($i = 0; $i < $pages_num; ++$i)
            {
                $num = $i + 1;
                if ($skip >= $ct && $skip < $ct + $perpage)
                    $num = "<b>$num</b>";
                $link = "<a href='index.php?${qs}skip=$ct&perpage=$perpage'>$num</a>";
                $res .= $link.' ';
                $ct += $perpage;
            }
            $res .= '</p>';
        }
    }
    else
    {
        if ($q->num_rows == 0)
            return $head.submitForm();
    } 
    
    return $head.$res;    
}

$search = trim(get($_GET, 'q'));

$doc->title = "View submissions";
$doc->content .= searchForm();
$doc->content .= showLastSubmits();

?>
