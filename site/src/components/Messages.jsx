import React, { useState } from 'react';
import ReactDOM from 'react-dom/client';
import Modal from 'react-modal';
import Button from './Button';
import { LayoutParams } from '../config/LayoutParams';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleDown, faAngleUp } from '@fortawesome/free-solid-svg-icons';

Modal.setAppElement('#root');

// ========= Estilos comuns =========
const customStyles = {
   content: {
      minWidth: '350px',
      maxWidth: '500px',
      margin: '20% auto',
      borderRadius: '8px',
      padding: 0,
      maxHeight: '400px',
      height: 'fit-content',
      zIndex: 2001, // 👈 garante que o conteúdo fique no topo
   },
   overlay: {
      backgroundColor: 'rgba(0, 0, 0, 0.5)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      zIndex: 2000, // 👈 overlay acima dos modais normais (1050 no bootstrap)
   },
};


// ========= Função auxiliar para abrir modais =========
function openModal(Component, props) {
   return new Promise((resolve) => {
      const container = document.createElement('div');
      document.body.appendChild(container);

      const root = ReactDOM.createRoot(container);

      const handleClose = () => {
         setTimeout(() => {
            root.unmount();
            container.remove();
            resolve();
         }, 0);
      };

      root.render(<Component {...props} onClose={handleClose} />);
   });
}

// ========= Exportar funções globais =========
export const showInfo = (text, title = 'Informação') => {
   return openModal(ModalInfo, { text, title });
};

export const showError = (text, details, stack, title = 'Ops!') => {
   return openModal(ModalError, { text, details, stack, title });
};

export const showConfirm = (text, onConfirm, onCancel, confirmText, cancelText, title = 'Confirmação') => {
   return openModal(ModalConfirm, { text, onConfirm, onCancel, confirmText, cancelText, title });
};

// ========= Componentes de Modal =========

const ModalInfo = ({ title, text, onClose }) => (
   <Modal isOpen={true} onRequestClose={onClose} style={customStyles}>
      <div className='modal-content'>
         <div className='modal-header' style={{ backgroundColor: LayoutParams.colors.corDoTemaPrincipal }}>
            <div className='modal-title h4' style={{ color: LayoutParams.colors.corSecundaria, padding: 10 }}>
               {title}
            </div>
         </div>
         <div className='modal-body' style={{ padding: '16px', whiteSpace: 'break-spaces' }}>
            {text}
         </div>
         <div className='modal-footer' style={{ padding: '12px', textAlign: 'right' }}>
            <Button text='OK' style={{ width: '130px' }} onClick={onClose} />
         </div>
      </div>
   </Modal>
);

const ModalError = ({ title, text, details, stack, onClose }) => {
   const [showDetails, setShowDetails] = useState(false);

   return (
      <Modal isOpen={true} onRequestClose={onClose} style={customStyles}>
         <div className='modal-content'>
            <div className='modal-header' style={{ backgroundColor: LayoutParams.colors.corDoTemaPrincipal }}>
               <div className='modal-title h4' style={{ color: LayoutParams.colors.corSecundaria, padding: 10 }}>
                  {title}
               </div>
            </div>
            <div className='modal-body' style={{ padding: '16px', whiteSpace: 'break-spaces' }}>
               <div>{text}</div>
               {(details || stack) && (
                  <div style={{ marginTop: '10px' }}>
                     <div style={{ textAlign: 'center' }}>
                        <FontAwesomeIcon
                           icon={showDetails ? faAngleUp : faAngleDown}
                           style={{ cursor: 'pointer', fontSize: 20, color: 'gray' }}
                           onClick={() => setShowDetails(!showDetails)}
                        />
                     </div>
                     {showDetails && (
                        <div style={{ marginTop: '8px' }}>
                           {details && <div style={{ marginBottom: '6px' }}>{details}</div>}
                           {stack && (
                              <pre style={{ fontSize: 10, border: '1px solid gray', borderRadius: 5, padding: '6px' }}>
                                 {stack}
                              </pre>
                           )}
                        </div>
                     )}
                  </div>
               )}
            </div>
            <div className='modal-footer' style={{ padding: '12px', textAlign: 'right' }}>
               <Button text='OK' style={{ width: '130px' }} onClick={() => onClose()} />
            </div>
         </div>
      </Modal>
   );
};

const ModalConfirm = ({ title, text, onClose, onConfirm, onCancel, confirmText, cancelText }) => (
   <Modal isOpen={true} onRequestClose={onClose} style={customStyles}>
      <div className='modal-content'>
         <div className='modal-header' style={{ backgroundColor: LayoutParams.colors.corDoTemaPrincipal }}>
            <div className='modal-title h4' style={{ color: LayoutParams.colors.corSecundaria, padding: 10 }}>
               {title}
            </div>
         </div>
         <div className='modal-body' style={{ padding: '16px', whiteSpace: 'break-spaces' }}>
            {text}
         </div>
         <div className='modal-footer' style={{ padding: '12px', textAlign: 'right' }}>
            <Button
               text={cancelText || 'Cancelar'}
               onClick={() => {
                  if (onCancel) onCancel();
                  onClose();
               }}
               style={{ width: '130px', marginRight: '8px' }}
            />
            <Button
               text={confirmText || 'Confirmar'}
               onClick={() => {
                  if (onConfirm) onConfirm();
                  onClose();
               }}
               style={{ width: '130px' }}
            />
         </div>
      </div>
   </Modal>
);
