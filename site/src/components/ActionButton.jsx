import React, { useState } from 'react';
import { LayoutParams } from './../config/LayoutParams';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export const ActionButton = ({
   icon,
   title,
   onClick,
   notificationCount,
   height,
   width,
   borderRadius,
   selected,
   light,
   circle,
   fullSelect,
   disableFullPaintOnHonver,
   disableHoverEfects,
   size,
}) => {
   const [hovered, setHovered] = useState(false);

   const truncateDecimals = function (number, digits) {
      var multiplier = Math.pow(10, digits),
         adjustedNum = number * multiplier,
         truncatedNum = Math[adjustedNum < 0 ? 'ceil' : 'floor'](adjustedNum);

      return truncatedNum / multiplier;
   };

   const getNotificationCountDescription = () => {
      let result = notificationCount;
      if (notificationCount >= 1000000) {
         result = `${truncateDecimals(notificationCount / 1000000, 1)}M`;
      } else if (notificationCount >= 10000) {
         result = `${truncateDecimals(notificationCount / 1000, 0)}k`;
      } else if (notificationCount >= 1000) {
         result = `${truncateDecimals(notificationCount / 1000, 0)}k`;
      }
      return result;
   };

   const getPrimaryColor = () => {
      return light ? LayoutParams.colors.corSecundaria : LayoutParams.colors.corDoTemaPrincipal;
   };

   const getSecondaryColor = () => {
      return light ? LayoutParams.colors.corDoTemaPrincipal : LayoutParams.colors.corSecundaria;
   };

   return (
      <div
         style={{
            display: 'table-cell',
            height: height ? height : null,
            width: circle ? height : width ? width : 60,
            minWidth: circle ? height : width ? width : 60,
            borderRadius: circle ? height / 2 : borderRadius,
            fontSize: size ?? 22,
            whiteSpace: 'nowrap',
            cursor: 'pointer',
            textAlign: 'center',
            color:
               (hovered && !disableFullPaintOnHonver) || (selected && fullSelect)
                  ? getPrimaryColor()
                  : getSecondaryColor(),
            backgroundColor:
               (hovered && !disableFullPaintOnHonver) || (selected && fullSelect)
                  ? getSecondaryColor()
                  : getPrimaryColor(),

            borderBottom: circle
               ? null
               : '3px solid ' +
                 (selected
                    ? hovered && !disableHoverEfects
                       ? getSecondaryColor()
                       : getSecondaryColor()
                    : hovered && !disableHoverEfects
                    ? getSecondaryColor()
                    : getPrimaryColor()),
         }}
         onMouseEnter={() => setHovered(true)}
         onMouseLeave={() => setHovered(false)}
         title={title}
         onClick={onClick}
      >
         {notificationCount > 0 && (
            <div
               style={{
                  height: 22,
                  width: 22,
                  borderRadius: 11,
                  backgroundColor: hovered ? getPrimaryColor() : getSecondaryColor(),
                  color: hovered ? getSecondaryColor() : getPrimaryColor(),
                  borderColor: hovered ? getSecondaryColor() : getPrimaryColor(),
                  borderStyle: 'solid',
                  borderWidth: 1,
                  position: 'fixed',
                  fontSize: 10,
                  marginLeft: 22,
                  marginTop: -4,
                  display: 'flex',
               }}
            >
               <div style={{ margin: 'auto' }}>
                  <Text>{getNotificationCountDescription()}</Text>
               </div>
            </div>
         )}
         <div style={{ margin: 'auto', height: '100%', width: '100%', display: 'flex' }}>
            <FontAwesomeIcon icon={icon} style={{ margin: 'auto' }} />
         </div>
      </div>
   );
};
