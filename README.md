# Teams Bot - Project Folder Extension
## Problem
Currently, there is no supported way of sharing links to the BC server or ProjectWise server through Microsoft Teams. 
And as we transition towards that platform and away from standard methods of sharing these links, this pitfall could impede productivity.

## Solution
The solution outlined below is a Teams App which uses Microsoft's Bot Framework to allow users to input server links and be
returned a card with a button that can open the path. The backend to this request is an API exposed by an ASP.NET Core Application
running in Azure currently which processes the link, creates the card, and sends it back to Teams. 

![Messaging Extension](/images/extension.png)
![Returned Card](/images/edit.png)
![Sent Card](/images/sent.png)

This solution was, unfortuantely, not as straightforward as I had thought it would be. The Bot Framework SDK has classes to create 
these card objects and one of the parameters they can take is a URL which can be opened on click. For security reasons, I'm guessing, 
any URIs of the form `file://` or `pw://` are censored and transformed before the card object is delivered. 
Only `http://` and `https://` links seem to be valid.

So in order to serve an `http://` link and have it open a `file://` path, 
I created a database for storing the links and generate a UID for each request that comes through. 
In the response is an `https://` link of the form 
[https://bc-teams-bot.azurewebsites.net/link/6ac144cd-930b-4654-98c7-cd027b98f540](https://bc-teams-bot.azurewebsites.net/link/6ac144cd-930b-4654-98c7-cd027b98f540) 
which, in this case, has a permanent redirect to 
`file://bcphxfp01/Projects/Phoenix, City of/152640 - Zone 3D-4A Improvement Prog Mgr/Calcs/Model/04_Deliverables/4_2_Memos/Ops-SOP/`.

## Caveats
The **biggest** caveat with this design is that opening `file://` links from an external website is expressely prohibited in Chrome. 
Only IE allows this behavior. Meaning, if you open these links in Chrome, they will not do anything. 
Moreover, this only works in IE when the domain initiating the request (in this case `bc-teams-bot.azurewebsites.net`) is listed in 
IE's Trusted Sites.

![Trusted Sites](/images/trusted_sites.png)

So, it would require that all requests to this domain made in Chrome are forwarded to IE. 
Unfortunately, the only way around this I could see is Microsoft officially supporting opening `file://` links (and `pw://`) 
links from a Teams card. And so far, I haven't found a means of making that happen, but where there's a will, there's a way!
