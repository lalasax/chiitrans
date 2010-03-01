<?php

function constructSubmitForm($id)
{
    return "
        <form id='newsubmit' method='post' action='submit.php'>
            <div class='fields'>
            <label>Submit your translation:<br>
            <div id='error'></div>
            <textarea name='trans'></textarea>
            </label><br>
            <label>Comment (optional):<br>
            <textarea name='comment'></textarea>
            </label><br>
            </div>
            <input type='submit' />
            <input type='hidden' name='from' value='site' />
            <input type='hidden' name='src_id' value='$id' />
        </form>
        <div>
        <button class='revert'>Delete translation</button>
        <form class='revertform' method='post' action='submit.php'>
        <div class='fields'>
        <label>Why do you want to delete the translation?<br>
        <textarea name='comment'></textarea>
        </label><br>
        </div>
        <input type='submit' value='Delete translation' />
        <input type='hidden' name='src_id' value='$id' />
        <input type='hidden' name='revert_to' value='-1' />
        <input type='hidden' name='from' value='site' />
        </form>
        </div>
    ";
}

function displayNotExist()
{
    global $doc;
    
    $doc->content = "<p id='error'>Error: translation does not exist.</p>";
}

function displayTranslation($id, $src)
{
    global $doc, $table;
    
    $src = e($src);
    $res = "
        <div id='view'>
        <div class='text'>$src</div>
        <ol id='revisions'>";
    $qq = "
        select t1.translation as translation, t1.revision as revision, t1.revert_to as revert_to, t1.comment as comment, t1.user_id as user_id, if(t1.user_id = 0, ip.id, u1.name) as name, unix_timestamp(t1.date) as _date
        from $table[translation] t1 left join $table[ip_data] ip on t1.user_ip = ip.ip left join $table[user] u1 on t1.user_id = u1.id
        where t1.source_id = ? order by t1.id desc;
    ";
    $q = query($qq, $id);
    $i = 0;
    while ($row = $q->fetch(MYSQL_ASSOC))
    {
        ++$i;
        $name = constructName($row['user_id'], $row['name']);
        $comment = $row['comment'] ? "<div class='comment'>".f($row[comment]).'</div>' : "";
        if ($row['revert_to'] == -1)
        {
            $revtype = 'Deleted';
            $text = '';
        }
        else
        {
            $text = "<div class='text'>".f($row['translation'])."</div>"; 
            if ($row['revert_to'] != 0)
            {
                $revtype = "Reverted to revision <a href='#$row[revert_to]'>$row[revert_to]</a>";
            }
            else
            {
                $revtype = "Submitted";
            }
        }
        $date = formatDate($row['_date']);
        if ($i > 1 && $row['revert_to'] != -1)
            $revert = "
                <div style='display: inline'>
                <button class='revert'>Revert to this</button>
                <form class='revertform' method='post' action='submit.php'>
                <div class='fields'>
                <label>Why do you want to revert the translation?<br>
                <textarea name='comment'></textarea>
                </label><br>
                </div>
                <input type='submit' value='Revert translation' />
                <input type='hidden' name='src_id' value='$id' />
                <input type='hidden' name='revert_to' value='$row[revision]' />
                <input type='hidden' name='from' value='site' />
                </form> 
                </div> 
            ";
        else
            $revert = "";
        $res .= "
            <a name='$row[revision]'></a>
            <li value='$row[revision]'>
            $revtype by $name on $date. $revert
            $text
            $comment
            </li>
        ";
        if ($i == 1)
            $res .= "</ol>".constructSubmitForm($id)."<ol id='revisions'>";
    }
    $res .= "</ol>";
    if ($i == 0)
        $res .= constructSubmitForm($id);
    $res .= "</div>";
    $res .= "
        <script>
        function toggleRevertForm(ev)
        {
            $(this).parent().find('form.revertform').toggle();
        }
        
        $(function()
        {
            $('button.revert').click(toggleRevertForm);
        });
        </script>
    ";
    $doc->content = $res;
}

$id = (int)get($_GET, 'id');
$doc->title = "View translation #$id";
if (!$id)
{
    displayNotExist();
}
else
{
    $src = queryGetString("select source from $table[source] where id = ?", $id);
    if (!$src)
    {
        displayNotExist();
    }
    else
    {
        displayTranslation($id, $src);
    }
}

?>
