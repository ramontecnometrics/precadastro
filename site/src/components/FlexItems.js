import React from "react";

export const FlexRow = ({ children, width, style }) => {
   const defaultStyle = { display: "flex", flexDirection: "row", width: width ? width : "100%" };

   return <div style={{ ...defaultStyle, ...style }}>{children}</div>;
};

export const FlexCol = ({ children, width, style }) => {
   const defaultStyle = { display: "table-cell", width: width ? width : "100%" };
   return <div style={{ ...defaultStyle, ...style }}>{children}</div>;
};
