import React from 'react';
import '../../../contents/css/login-logo.css';

export default function LoginLogo(props) {
    return (
        <img
            src={require('../../../contents/img/logo.svg')}
            alt="Logo"
            className={'login-logo' + (props.className ? ' ' + props.className : '')}
            style={props.style}
        />
    );
}
