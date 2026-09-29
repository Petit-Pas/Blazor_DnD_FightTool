# Blazor_DnD_FightTool
Updated version of DungeonAndDragonFightingTools to use Maui X Blazor technology

Also .Net Core and better infrastructure overall

# Bugs
- fighters are ordered in descending order when having the same name
- redo undo redo undo of adding fighters => the fighters cards do not refresh automatically

- LogToken and LogTokenParser might not be defined in the proper project

- add same monsters => attack => click on different cards => all HP bars are the same.

# Mediator
- Improvement possible with a command to fully clear redo history.



/bmad-build spec 005, user story 5
avant de démarrer la user story 6: 
- verifier que des tests ne doivent pas être split
- UITests UI/ folder does not exist, but we still need to make an easy differencniation between UI/ anc Components/ 
- can we make a sequential be skipped if they are not ran in the proper order? 
- the actual mock of the UI components all live in the same place under UITestFramework
- new rules have been set for UITests: empty static then internal class for each test scenario, this has not been applied so far.