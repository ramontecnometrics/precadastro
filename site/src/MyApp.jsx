import React, { useRef } from 'react';
import { useReactToPrint } from 'react-to-print';

const MyApp = () => {
   const printRef = useRef(null);
   const handlePrint = useReactToPrint({
      contentRef: printRef,
   });

   return (
      <div>
         <div ref={printRef}>
            <h1>Content to be printed</h1>
            <p>This is some text that will appear in the printout.</p>
         </div>
         <button onClick={handlePrint}>Print</button>
      </div>
   );
};

export default MyApp;
