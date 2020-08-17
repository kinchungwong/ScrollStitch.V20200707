# ScrollStitch.V20200707.NUnitTests.Spatial

NUnit-based unit tests for the ScrollStitch.V20200707.Spatial namespace.

---

##### Current (interim) status and reasons

Currently, the newly added tests focuses on RectTree (and RectTreeDev), mainly because 
this area of code is much more complicated (in terms of logic and correctness) than the
rest of the entire project.

Even mundane functions, such as computing the intersections of rectangles, or the use
of bit mask accelerations for rectangle intersection tests, were found to harbor bugs
that could throw off experienced programmers.

The actual implementation of enhanced RectTree (notably FastRectNode) dispeled a false
dichotomy between correctness issues and performance issues. There, many subtle code
changes led to an explosion in resource usage. It turns out that the lazy-eager behaviors
of a data structure such as RectTree are of paramount importance in terms of both 
theoretical correctness (the theoretical ability to give result without regard to time 
bounds or resource bounds) and practical correctness (of computing the expected result 
before the end of the universe is reached).

---

### Source code layout

- First level source subfolder is organized by ...
- Second level source files may correspond to either a class or a single utility function.
