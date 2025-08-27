import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { LayoutParams } from '../../config/LayoutParams';
import { faExternalLink } from '@fortawesome/free-solid-svg-icons';

export default function BotaoVisualizarItemDeCadastro({ onClick, title, size = 23 }) {
    return (
       <FontAwesomeIcon
          className='custom-hover'
          title={title}
          style={{
             fontSize: size,
             paddingTop: 2,
             marginLeft: 3,
             marginRight: 3,
             color: LayoutParams.colors.corDosBotoesDoFormularioPadrao,
          }}
          cursor='pointer'
          icon={faExternalLink}
          onClick={onClick}
       />
    );
 };