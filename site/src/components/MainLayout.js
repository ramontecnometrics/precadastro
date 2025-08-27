import React, { useState, useEffect, useRef } from 'react';
import styled from 'styled-components';
import { Link } from 'react-router-dom';
import { LayoutParams } from './../config/LayoutParams';
import { OverlayTrigger, Popover } from 'react-bootstrap';
import IconButton from '../components/IconButton';
import { faBars, faSignOutAlt, faUserCircle, faUser, faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import SimpleBar from 'simplebar-react';
import 'simplebar-react/dist/simplebar.min.css';
import './../contents/css/home-logo.css';
import Avatar from './Avatar';
import Text from './Text';
import BottomMenuLogo from './BottomMenuLogo';
import sessionManager from '../SessionManager';

// ================= MainLayout =================

export default function MainLayout({ children, menuItems }) {
   const [showMenu, setShowMenu] = useState(true);

   const login = sessionManager.getLogin();

   const toggleMenu = () => {
      setShowMenu((prev) => {
         const newValue = !prev;
         const evt = new CustomEvent('ontogglemenu', { detail: { showMenu: newValue } });
         window.dispatchEvent(evt);
         return newValue;
      });
   };

   return (
      <div style={{ position: 'fixed', height: '100%', width: '100%' }}>
         <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
            <NavigationBar toggleMenu={toggleMenu} showMenu={setShowMenu} login={login} />
            <div
               style={{
                  display: 'flex',
                  flexDirection: 'row',
                  flexBasis: '100%',
                  overflow: 'hidden',
                  maxHeight: '100%',
                  padding: 0,
               }}
            >
               <SideMenu menuItems={menuItems} showMenu={showMenu} />
               <div style={{ height: '100%', width: '100%', overflow: 'auto', display: 'flex', fontSize: 13 }}>
                  <div id='div-conteudo' style={{ overflow: 'auto', width: '100%' }}>
                     {children}
                  </div>
               </div>
            </div>
         </div>
      </div>
   );
}

// ================= NavigationBar =================

function NavigationBar({ toggleMenu, showMenu, login }) {
   const navRef = useRef(null);

   useEffect(() => {
      const handleResize = () => {
         if (navRef.current) {
            const shouldHide = navRef.current.offsetWidth < 900;
            showMenu((prev) => (prev !== !shouldHide ? !shouldHide : prev));
         }
      };
      window.addEventListener('resize', handleResize);
      handleResize();
      return () => window.removeEventListener('resize', handleResize);
   }, [showMenu]);

   return (
      <div
         style={{
            backgroundColor: LayoutParams.colors.corDoTemaPrincipal,
            color: LayoutParams.colors.corDoTemaPrincipal,
            display: 'flex',
            flexDirection: 'row',
            width: '100%',
         }}
         ref={navRef}
      >
         <div
            style={{
               width: 70,
               fontSize: 30,
               textAlign: 'center',
               cursor: 'pointer',
               paddingTop: 10,
               color: LayoutParams.colors.corSecundaria,
            }}
         >
            <IconButton icon={faBars} onClick={toggleMenu} />
         </div>
         <div
            style={{
               flex: 1,
               padding: 10,
               fontSize: 20,
               color: LayoutParams.colors.corSecundaria,
               whiteSpace: 'nowrap',
               overflow: 'hidden',
            }}
         >
            {login?.empresa && <Text>{login.empresa.nomeFantasia}</Text>}
         </div>
         <div
            style={{
               position: 'fixed',
               top: 6,
               right: 6,
               fontSize: 12,
               display: 'flex',
               flexDirection: 'row-reverse',
               cursor: 'pointer',
            }}
         >
            <AccountMenu login={login} />
         </div>
      </div>
   );
}

function AccountMenu({ login }) {
   return (
      <OverlayTrigger
         trigger='click'
         placement='bottom'
         rootClose={true}
         overlay={
            <Popover id='popover-basic'>
               <Popover.Header>
                  <div className='noselect' style={{ color: LayoutParams.colors.corDoTemaPrincipal }}>
                     {login ? (login.primeiroNome ? login.primeiroNome : login.nomeDeUsuario) : null}
                  </div>
               </Popover.Header>
               <Popover.Body style={{ padding: 0, display: 'flex', flexDirection: 'column' }}>
                  <MenuLink to='/account' icon={faUser} text='Meus dados' />
                  <MenuLink to='/about' icon={faInfoCircle} text='Sobre' />
                  <MenuLink to='/logoff' icon={faSignOutAlt} text='Sair' />
               </Popover.Body>
            </Popover>
         }
      >
         <div className='noselect' style={{ color: LayoutParams.colors.corSecundaria }}>
            {login?.foto ? (
               <Avatar image={login.foto} readOnly={true} width={35} />
            ) : (
               <IconButton icon={faUserCircle} style={{ fontSize: 35, paddingTop: 3 }} />
            )}
         </div>
      </OverlayTrigger>
   );
}

// ================= SideMenu =================

function SideMenu({ menuItems, showMenu, level = 0 }) {
   return (
      <SideMenuStyled $showMenu={showMenu} id='sideMenuStyled'>
         <div
            style={{
               width: '100%',
               height: '100%',
               overflow: 'hidden',
               fontSize: 13,
               backgroundColor: LayoutParams.colors.corDoTemaPrincipal,
            }}
         >
            <SimpleBar style={{ maxHeight: '100%' }}>
               {menuItems
                  .filter((item) => item.enabled)
                  .map((item, index) => {
                     return (
                        <SideMenuGroup
                           key={index}
                           icon={item.icon}
                           label={item.label}
                           subMenu={item.subMenu}
                           level={level}
                           route={item.route}
                        />
                     );
                  })}
            </SimpleBar>
         </div>
         <BottomMenuLogo />
      </SideMenuStyled>
   );
}

function SideMenuGroup({ icon, label, subMenu, level, route }) {
   const [isCollapsed, setCollapsed] = useState(true);
   level++;
   return (
      <div>
         <div
            className='menu-item'
            role='button'
            aria-expanded={!isCollapsed}
            onClick={() => setCollapsed(!isCollapsed)}
            style={{ paddingLeft: (level - 1) * 12 }}
         >
            <Link to={route}>
               <div className="menu-item-content">
                  <div style={{ display: 'inline-flex' }}>
                     <div className='menu-icon'>
                        <IconButton icon={icon} />
                     </div>
                     <Text style={{ margin: 'auto' }}>{label}</Text>
                  </div>
               </div>
            </Link>
         </div>

         {!isCollapsed &&
            subMenu &&
            subMenu
               .filter((item) => item.enabled)
               .map((item, index) => {
                  return (
                     <SideMenuGroup
                        key={index}
                        icon={item.icon}
                        label={item.label}
                        subMenu={item.subMenu}
                        level={level}
                        route={item.route}
                     />
                  );
               })}
      </div>
   );
}

// ================= Reusable MenuLink =================

function MenuLink({ to, icon, text, isSubItem, fontWeight }) {
   return (
      <Link to={to} $isSubItem={isSubItem}>
         <div style={{ display: 'inline-flex' }}>
            <div className='menu-icon'>
               <IconButton icon={icon} />
            </div>
            <div style={{ display: 'flex' }}>
               <Text style={{ margin: 'auto', fontWeight }}>{text}</Text>
            </div>
         </div>
      </Link>
   );
}

// ================= Styles =================

const SideMenuStyled = styled.nav`
   color:  ${() => LayoutParams.colors.corSecundaria};
   background-color:  ${() => LayoutParams.colors.corDoTemaPrincipal};
   display: ${(props) => (props.$showMenu ? 'flex' : 'none')};
   flex-direction: column;
   min-width: 200px;   
   overflow-y: hidden;
   border-top: 1px solid ${() => LayoutParams.colors.corSecundaria};
      
   .menu-item {

      a {
         text-decoration: none;
      }

      .menu-item-content {
         padding: 6px 2px 6px 5px;            
      }

      &:hover {
         color: ${() => LayoutParams.colors.corDoTemaPrincipal};
         background-color:  ${() => LayoutParams.colors.corSecundaria};
      }
   }

   .menu-icon {
      text-align: center;
      width: 40px;
      font-size: 25px;
   }
}
`;
