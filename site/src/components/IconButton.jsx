import React from 'react'; 
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export default function IconButton(props) {
   return <FontAwesomeIcon {...props} cursor={props.cursor ? props.cursor : 'pointer'} />;
}
