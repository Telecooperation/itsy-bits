include <methods.scad>;
//init input
model = 0;

/*
 Anzeige
*/

/*getScaledVersions(factors, space)
factors: Array von Skalierungsfaktoren. 
        Die Basismodell sind ungefähr so gehalten, dass der Durchmesser 1mm beträgt.
        Somit gibt der Faktor eine gute Abschätzung (in mm) ab, wie groß die Modell sein werden.
space: Abstand zwischen zwei Objekten (Mitte -> Mitte)
*/
getScaledVersions([15,20], 35, model) {
/*
    _triangle();
    _arrow();
    
    _circle();
    _star();
    
    _square();
    _notchedSquare();
	*/
	
    _triangleCut();    
    _starNotch();   
    _squareCut();    
    _trapez();    
    _heart();    
    _circleCut();   
    _cross();    
    _hexa();    
    _arrow();    
    _moon();
}
    