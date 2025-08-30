import React from 'react';
import '../contents/css/home-logo.css';

export default function LogoHeaderHome(props) {
    return (
        <img
            src={require('../contents/img/logo.svg')}
            alt="Logo"
            className={'home-logo' + (props.className ? ' ' + props.className : '')}
            style={props.style}
        />
    );
}
