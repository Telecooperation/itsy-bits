include <methods.scad>;
/*
 Anzeige
*/

/*getScaledVersions(factors, space)
factors: Array von Skalierungsfaktoren. 
        Die Basismodell sind ungefähr so gehalten, dass der Durchmesser 1mm beträgt.
        Somit gibt der Faktor eine gute Abschätzung (in mm) ab, wie groß die Modell sein werden.
space: Abstand zwischen zwei Objekten (Mitte -> Mitte)
model: index des anzuzeigenden Elements. 0 für alle
*/
$fn = 50;

rotate([90,0,0])
getScaledVersions([35], 0, 0) {    
    //_arrow();
    //_square();
    
    //    _triangleCut();    
    //    _starNotch();   
    //    _squareCut();    
    //    _trapez();    
    //    _heart();    
    //    _circleCut();   
        //_cross();    
    //   _hexa();    
    //_arrow();    
    _moon();
}