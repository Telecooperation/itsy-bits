FOR /L %%A IN (1,1,24) DO (
  openscad -o %%A.stl -D model=%%A run.scad
)