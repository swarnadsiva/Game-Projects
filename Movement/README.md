<h1 id="movement-a.k.a.-pajama-man-wandering-a-maze-with-a-rifle">Movement (A.K.A. Pajama Man Wandering a Maze With a Rifle)</h1>
<dl>
<dt>Developer</dt>
<dd>Durga Sivamani</dd>
<dt>Composer / Sound Designer</dt>
<dd>Jake Kargl</dd>
</dl>
<p>I recently attended a game jam and met an awesome sound designer and composer (who just happens to be a grad student like me). After the jam, we decided to work on a small project together to refine our game dev skills.</p>
<blockquote>
<p>Current progress:</p>
</blockquote>
<p><img src="Gifs/movement_status_11-19-19.gif" alt=""></p>
<p>As you can see, this game is still a-cookin’ and needs a lot of work to become a polished product. Although I would love to take credit for the amazing player character model and animations, they are actually from Unity 3D’s free <a href="https://learn.unity.com/project/survival-shooter-tutorial">Survival Shooter tutorial</a> asset package.</p>
<p><em>Expect to see lots of changes upon new visits to this page!</em></p>
<h2 id="game-features">Game Features</h2>
<p>There are several features I’ve been wanting to implement using solid software design and architecture <sup><a href="#fn1">1</a></sup>. See below for details on what I’ve already implemented / will be implementing soon.</p>
<h3 id="procedural-maze-creation">Procedural Maze Creation</h3>
<p>I designed a series of scripts (under the “Level Generation” folder) that design a new maze every time a level is created. There are some exposed options like <em>Tile Size</em>, <em>Border Thickness</em>, <em>Maze Height</em>, and <em>Maze Width</em> that allow developers to specify general properties about the maze using the Unity Editor <sup><a href="#fn2">2</a></sup>.</p>
<p>The scripts work as follows:</p>
<ol>
<li>The <strong>LevelGenerator</strong> in the scene creates a new <strong>Maze</strong> object with the specified parameters above.</li>
<li>The <strong>Maze</strong> object creates a 2D array of <strong>AbstractTile</strong> objects (instantiated as <strong>FloorTile</strong>, <strong>WallTile</strong> and <strong>EmptyTile</strong>).</li>
<li>The script traverses the array and populates cells, i.e., <strong>WallTile</strong> objects along the edge of the maze according to <em>Border Thickness</em>, <strong>FloorTile</strong> objects in a basic grid pattern according to the <em>Maze Height</em> and <em>Maze Width</em>. Everything else is an <strong>EmptyTile</strong>.</li>
<li>Then the script uses a recursive minimum spanning tree algorithm to traverse the <strong>FloorTile</strong> objects like a graph <sup><a href="#fn3">3</a></sup> and make a walkable path through the maze. Random <strong>FloorTile</strong> objects are created in between the paths to create loops in the maze (for added playing difficulty).</li>
<li>Finally, every <strong>EmptyTile</strong> object is converted to a <strong>WallTile</strong> to create divisions and complete the maze.</li>
</ol>
<blockquote>
<p>See this in action! Every time the scene loads (here I’m doing it manually), a new maze is created!</p>
</blockquote>
<p><img src="Gifs/maze_generation.gif" alt=""></p>
<h3 id="mesh-generation">Mesh Generation</h3>
<p>This works in conjunction with the procedural maze generation outlined above. Each time a maze is created, the meshes for each portion of the maze (floor, border walls, internal walls) are created by manually setting vertices, triangles, UVs and normals. This was a painstaking process to get right, and required a lot of conditional checking to ensure meshes weren’t being made when you wouldn’t see them (ex: in between walls).</p>
<p>Materials for each distinct mesh are set as options above, and sent through the <strong>LevelGenerator</strong> to the <strong>Maze</strong> object upon instantiation.</p>
<h3 id="multiplayer-options">Multiplayer Options</h3>
<p>I am planning on creating a playable PvP option where players can compete against one another to see who can find their way out of the maze first. I haven’t done this before, so I’m curious to see Unity’s support for multiplayer systems.</p>
<h3 id="everything-else">Everything Else</h3>
<p>Like I said above… this game’s still a-cookin’! Lots more features will be coming soon, like intelligent enemy AI, portals to leave the maze, keys to open the portals, and more.</p>
<h4 id="footnotes">Footnotes</h4>
<hr>
<p><em>Written with <a href="https://stackedit.io/">StackEdit</a>.</em></p>
<p><a id="fn1">1.</a> I’m going through this book, <a href="https://gameprogrammingpatterns.com/">Game Programming Patterns</a> by Robert Nystrom, and it’s changing my game programming life.</p>
<p><a id="fn2">2.</a> These can also be accessed and set programmatically in code.</p>
<p><a id="fn3">3.</a> I say “like a graph” because I am not adhering to a strict graph data structure in this project. The tiles are stored in a 2D array, so even though they have a Dictionary collection of their neighboring tiles and relative positions, this is not what I am solely depending on to construct the maze.</p>

