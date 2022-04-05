module Star(points, outer, inner) {
	
	// polar to cartesian: radius/angle to x/y
	function x(r, a) = r * cos(a);
	function y(r, a) = r * sin(a);
	
	// angular width of each pie slice of the star
	increment = 360/points;
	
	union() {
		for (p = [0 : points-1]) {
			
			// outer is outer point p
			// inner is inner point following p
			// next is next outer point following p

			assign(	x_outer = x(outer, increment * p),
					y_outer = y(outer, increment * p),
					x_inner = x(inner, (increment * p) + (increment/2)),
					y_inner = y(inner, (increment * p) + (increment/2)),
					x_next  = x(outer, increment * (p+1)),
					y_next  = y(outer, increment * (p+1))) {
				polygon(points = [[x_outer, y_outer], [x_inner, y_inner], [x_next, y_next], [0, 0]], paths  = [[0, 1, 2, 3]]);
			}
		}
	}
}

module stamp(factor){
    scale([factor,factor,1]) linear_extrude(6) children(0);
}

module figure(factor){
    /*union(){
        translate([0,0,29]) sphere(7);
        translate([0,0,5]) cylinder(20,4,4);
        stamp(factor) children(0);
    }*/
	stamp(factor) children(0);
}

module negative(){
    difference(){
        cylinder(30,16,2);
        cap();
    }
}

module getScaledVersions(factors, space,model=0){
    for ( i= [0:$children-1])
    { 
        /*for (j = [0:len(factors)-1])
        {
            if(model==0 || model == ((i*len(factors))+(j+1))){
                translate([i*space,0,0]) {
                    translate([0,j*space,0]) figure(factors[j]) children(i);
                }
            }
        }*/
		children(i);
    }
}

module flat_heart() {
  square(10);

  translate([5, 11, 0])
  circle(5);

  translate([11, 5, 0])
  circle(5);
}

/*********
* SHAPES *
*********/
module _triangle(){
    scale([0.12,0.12,0.12]) translate([-5,-5,0]) polygon([[0,0],[10,0],[5,10]]);
}
module _triangleCut(){
    scale([0.12,0.12,0.12]) translate([-5,-2.5,0]) polygon([[0,0],[3,5],[8,5],[10,0]]);
}


module _arrow(){
    scale([0.12,0.12,0.12]) translate([-5,-5,0]) polygon([[0,0],[5,2.5],[10,0],[5,10]]);
}
module _circle(){
    scale([0.01,0.01,0.01]) circle(55);
}
module _star(){
    scale([0.1,0.1,0.1]) Star(5,6.5,3.5);
}

module _starNotch(){    	
	scale([0.1,0.1,0.1]) 
	difference()  {
		Star(5,6.5,3.5);
		rotate([0,0,-18]) translate([-10,2.8,0]) square(30);		
	}
}

module _circleCut(){
   scale([0.1,0.1,0.1]) 
	difference()  {
		circle(5.3);
		rotate([0,0,-18]) translate([-10,2.8,0]) square(30);		
	}	
}

module _square(){
    square(1,true);
}
module _notchedSquare(){
    scale([0.1,0.1,0.1]) polygon([[-5,-5],[0,-2],[5,-5],[5,5],[-5,5]]);
}

module _squareCut(){
   scale([0.1,0.1,0.1]) translate([0.5,-1,0]) polygon([[-5,-5],[-1,-5],[5,1],[5,5],[-5,5]]);
}

module _trapez(){
    scale([0.1,0.1,0.1]) translate([-5,-3,0]) polygon([[-1,0],[5,7],[9,7],[9,0]]);
}

module _heart(){
   scale([0.06,0.06,0.06]) translate([-6,-5,0]) flat_heart();
}

module _cross(){
   translate([-0.7,-0.15,0]) scale([0.15,0.15,0.15]) {
   square([9,3]);
   translate([3,-3,0]) square([3,6]);
   }
}

module _hexa(){
    scale([0.12,0.12,0.12]) translate([-5,-3.5,0]) polygon([[3,0],[-1,3],[3,7],[8,7],[8,0]]);
}



module _arrow() {
  translate([-0.5,0,0]) scale([0.13,0.13,0.13]) {
translate([2,-5,0]) square([4,6]);
 polygon([[0,0],[8,0],[4,6]]);
}
}

module _moon() {
	//square([1,1,1]);
		
	translate([0.35,0,0]) scale([0.13,0.13,0.13]) {
	difference() {
		circle(5);
		translate([4,0,0]) circle(5);
	}
	}	
}

/*********
* SHAPES *
*********/
    