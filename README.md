# XRTerra_Paintball VR

VR mode
features:
teleportation
world space ui
snap turn
two handed gun
quest version

Additional Features:
splat mapping, toon shading, bottle and liquid shader, gun model, shoot fx, skybox shader, multiple paint colors, erasing paint, post processing, overlay camera, lightmaps, multiple targets (enemies) 

Details:
The splatmap is the same size as the lightmap 2048x2048 because they share the same uvs (UV1). They share uvs because I didn't want to generate them myself and because I can get raycasthit.lightmapcoord pretty easily.
However, I had to import my own simple plane and cube (because unity default uvs were causing issues), change all colliders to MeshColliders (because raycasthit.lightmapcoord only works like that) and set everything to lightmap static (otherwise they won't get a uv).
I was also having problems with scale in the uvs, some were bigger than others and were all squares instead of rectangular like they are in the scene. I had to get the gameobject scale for the face that was hit, but that didn't completely resolve the issue so paint splats on different objects may have different scales (especially the targets, but I'm calling that one a feature to encourage better aiming).
the original decal texture was used to give shape to the splats. I read the pixels (after scaling and randomly rotating coordinates for better visuals) from the splat texture and then wrote them to the paintmap.
the clear color is similar, only it removes paint instead of adding or replacing it.
In case we hit an interior corner, multiple rays are cast from the initial paintball impact, then creates multiple splats if it hits a different object.
It is possible to read values from the paintmap and then cause special effects, for example bouncing and speed like Portal 2's Gels or allow an ability like in Splatoon.

toongraph shader is used for all materials besides the bottle, it is what supports the paintmap and paint splatting
the bottle uses glassgraph shader for the outer glass and bottlegraph shader for the inner paint.
for the wobble effect I calculate the sin of the object's velocity in a script and set the wobble x and y shader parameters. Though I think the calculation is off somewhat because it doesn't always wobble physically correctly.

I had a little trouble building for windows as the paint map wasn't working and didn't cause any exceptions. I had to put the paintmap and splat texture in the Resources folder to tell unity to keep them as is, and I had to make sure read/write was enabled on my meshes or raycast won't give the lightcoord in the build.
I didn't build for mobile because I knew it was a poor fit with the high resolution lightmaps and all the shader effects, maybe next time.
