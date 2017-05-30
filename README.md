EasyMotion
==========

A vim / [sublime](https://github.com/tednaleid/sublime-EasyMotion) EasyMotion clone for Visual Studio. 


* You can search for segments longer than one char by pressing space. Each space increments segment length by one.

* If you query contains uppercase characters, search won't ignore case.

* Compatible with [VsVim](https://github.com/jaredpar/VsVim)

## Searching Example (single character)
The caret is at the end of the using block and you want to move it to the 'C' in 'Console.WriteLine.  

![Example Step 1](/images/example1.png)

Instead of moving your hands to the arrow keys or even worse, grabbing the mouse, simple initiate an easy motion search by pressing `Shift+Control+;`.  

![Example Step 2](/images/example2.png)

The editor adds a status line to let you know it's ready to search.  Type 'C' as this is the character we want to navigate to.

![Example Step 3](/images/example3.png)

The editor will replace all occurrences of 'C' with a new letter (A-Z).  At this point if you type any of the letters the caret will be moved to that location.  In this case you type 'I' to move to the correct occurrence of 'C'

![Example Step 4](/images/example4.png)

The caret is now at the start of 'Console.WriteLine' as we wanted at the start






