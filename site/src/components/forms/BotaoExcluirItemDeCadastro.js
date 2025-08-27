import { faTrashAlt } from '@fortawesome/free-regular-svg-icons';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { LayoutParams } from '../../config/LayoutParams';

export default function BotaoExcluirItemDeCadastro({ onClick, title, size = 23 }) {
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
         icon={faTrashAlt}
         onClick={onClick}
      />
   );
};
