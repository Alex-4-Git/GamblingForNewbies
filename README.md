# GamblingForNewbies
This repository is the source code of C# and ASP.NET project that we've done in 2014 summer, the website is a real time gaming platform for Rock-Paper-Scissors, where you can play game, discuss, and see your ranking score. I was responsible for the Ranking Controller.



# Xiaoyu's work(just copy from my final report), hope it can help recall some details.
1. I coded some parts of the forum section:
1) render the forum sections in the forum main page.
2) render all threads in each section, including title of the thread, author(username) of the thread, and the post date of the thread.Each username is a link to the author's profile page. In this page, there is a new thread button, logged in users can press this button(be directed to a new page) to post new thread to this section.

2. I coded some parts of the Gambling Hall section:
1) in the left hand side of the Gambling Hall, render all the current online users. Each username is a link to the author's profile page.
In User Table, there is an attribute: LastActiveTime, which will be updated every time a user log in, click a page link, post a new thread, add a reply to a thread, post a message to other users' wall, or play a game. This attribute will be used to determine whether a user is online or offline: if in last 30 seconds, the LastActiveTime is updated, which means the user did something in the website, the user is online. otherwise, the user is offline, so he or she will not appear in the online users section.
Used Ajax to update online users every 1 second and render it in the Gambling Hall Page.

2) added some logic to the game(user log): User table has these two columns: win and lose count. After each game, the win count of the winner will add one, and the lose count of the loser will add one. These user log will appear in the user profiles.

3. I coded some parts of the User Profile section:
1) I wrote some CSS code to change the appearance of the User Profile page.
2) I wrote some code to add Wall function in the User Profile: users can post messages to the wall of other users.
3) I wrote some code to add this: when a user is viewing his own profile, he/she can see a edit button below the discription part of his profile, which he can press and edit his discription. While viewing other user's profile, he cannot see the edit button.

