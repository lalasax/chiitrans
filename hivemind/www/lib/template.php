<?php

class Document
{
    public $template_name;
    private $template;
    public $title;
    public $content;
    private $vars;
    
    public function __construct($template_name)
    {
        $this->template_name = $template_name;
        $this->template = file_get_contents("templates/${template_name}.html");
        $this->title = "";
        $this->content = "";
        $this->vars = array();
    }

    private function replaceCallback($match)
    {
        $s = $match[1];
        if ($s == 'content')
            return $this->content;
        else if ($s == 'title')
            return $this->title;
        else if (isset($this->vars[$s]))
            return $this->vars[$s];
        else
            return "";
    }
    
    public function set($var, $value)
    {
        $this->vars[$var] = $value;
    }
    
    public function get($var)
    {
        return $this->vars[$var];
    }

    public function render()
    {
        return preg_replace_callback('/\{\{(\S*?)\}\}/', array(&$this, 'replaceCallback'), $this->template);
    }                                   
}

?>
