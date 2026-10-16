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
- some UI tests use css classes to find elements, should we not work with aria label instead? 
- some UI tests do something on the UI, then checks the UI impact of that something on the UI itself. Eg: when cliking on a fighter tile, it lights up. Technically correct, however it would be much better to test the following: 1) when I click a tile, it's selected (from UI to service). 2) when the service updates the selected fighter, the tile lights up (from service to UI). Doing from UI to UI is not really a unit test. Same with checking that redo changes the UI round number. In effect, redo should be tested towards Mediator History for instance. Or through an injected fake if possible.