The modals for user interactions have to be launched by a component in the render tree for some reasons. 
When I was starting them from a background service, no modal was ever visible. 

One way to fix this was to benefit from the partial component of the MainLayout (which registers to new user interaction requests) and launch the modals from there.
Of course, to clean the code maintainable, I do not want to have all the modals code in the MainLayout so I just extend the MainLayout partial class in separate files.