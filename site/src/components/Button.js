import React, { useState, useRef, forwardRef, useImperativeHandle } from 'react';
import { Button as BoottrapButton } from 'react-bootstrap';
import { LayoutParams } from '../config/LayoutParams';
import '../contents/css/button-input.css';

const Button = forwardRef(function Button(props, ref) {
  const [text, setText] = useState(props.text);
  const [disabled, setDisabled] = useState(false);
  const buttonRef = useRef(null);

  // expõe o método focus() para o pai
  useImperativeHandle(ref, () => ({
    focus: () => buttonRef.current && buttonRef.current.focus(),
  }));

  const defaultStyle = {
    backgroundColor: LayoutParams.colors.corDoTemaPrincipal,
    borderBlockColor: LayoutParams.colors.corSecundaria,
    color: LayoutParams.colors.corSecundaria,
    borderColor: LayoutParams.colors.corSecundaria,
    fontSize: 16,
    height: 36,
    textAlign: 'center',
    margin: 0,
    padding: '0 4px 0 4px',
    overflow: 'hidden',
  };

  const handleClick = (e) => {
    if (props.onClick) {
      setText(props.inProgressText);
      setDisabled(true);
      try {
        props.onClick(e);
      } finally {
        setText(props.text);
        setDisabled(false);
      }
      return;
    }

    if (props.onClickAsync) {
      setText(props.inProgressText);
      setDisabled(true);
      Promise.resolve(props.onClickAsync(e))
        .catch(() => {
          // erro já tratado por quem chamou, apenas reestabelece estado
        })
        .finally(() => {
          setText(props.text);
          setDisabled(false);
        });
    }
  };

  return (
    <BoottrapButton
      className={'button-input' + (props.className ? ' ' + props.className : '')}
      style={{ ...defaultStyle, ...props.style }}
      title={props.title}
      name={props.name}
      id={props.id}
      ref={buttonRef}
      disabled={disabled || props.disabled}      
      type={props.type || 'button'}
      onClick={handleClick}
    >
      <div style={{ display: 'flex', justifyContent: 'center', width: '100%', paddingLeft: 3, paddingRight: 3 }}>
        <div style={{ display: 'table-cell', width: props.icon && text ? 'fit-content' : '' }}>
          {props.icon}
        </div>
        {text && (
          <div
            style={{
              display: 'table-cell',
              width: text ? 'fit-content' : null,
              textAlign: 'left',
              paddingLeft: 5,
              justifyContent: 'center',
            }}
          >
            {text}
          </div>
        )}
      </div>
    </BoottrapButton>
  );
});

export default Button;
