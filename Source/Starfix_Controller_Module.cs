using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using KSP.IO;

namespace PFUtilityAddon
{
	public class FixedStar
	{
		public FixedStar( string IName, CustomStar ICStar)
		{
			Name = IName;
			CStar = ICStar;
		}
		
		public string Name;
		public CustomStar CStar;
	};
	
	public class Detector : MonoBehaviour
	{
		//bool Toggle;
		
		Dictionary< string, FixedStar> Stars = new Dictionary<string, FixedStar>();
		
		public void AddStar( string Name, CustomStar CStar )
		{
			Stars.Add( Name,  new FixedStar( Name, CStar) );
			
			try
			{
				CStar.SunlightEnabled( false );
			}catch{}
		}
		
		void Update()
		{
			if( Stars.Count <= 0 )
			{
				return;
			}
			
			//TODO: Fix support for binary+ star systems.
			foreach( FixedStar LoopedStar in Stars.Values )
			{
				//Grab altitude
				Vector3 pos = ScaledSpace.ScaledToLocalSpace( PlanetariumCamera.fetch.GetCameraTransform().position );
				Vector3 pos2;
				if( FlightGlobals.ActiveVessel != null )
				{
					pos2 = FlightGlobals.ActiveVessel.GetTransform().position;
				}
				double dist;
				
				dist = FlightGlobals.getAltitudeAtPos(pos, LoopedStar.CStar.sun);
				
				//In map mode?
				if( PlanetariumCamera.fetch.enabled == true )
				{
					if( dist < LoopedStar.CStar.sun.sphereOfInfluence )
					{
						if( LoopedStar.CStar.Toggle == true )
						{
							print ( "Scaled Enabling "+LoopedStar.Name+" Sun \n");
							LoopedStar.CStar.SunlightEnabled( true ); //Enable OUR stars light!
							
							Sun.Instance.SunlightEnabled( false );
							LoopedStar.CStar.Toggle = false;
							
							Planetarium.fetch.Sun = Utils.FindCB( LoopedStar.Name ); //Set the sun to OUR sun!
							
							foreach( ModuleDeployableSolarPanel panel in FindObjectsOfType( typeof( ModuleDeployableSolarPanel ) ) ) //Reboot solar panels
							{
								panel.OnStart( PartModule.StartState.None ); //todo: find out what the partstate does.
							}
						}
					}
					else if( LoopedStar.CStar.Toggle == false )
					{
							print ( "Scaled Disabling "+LoopedStar.Name+" Sun\n" );
							LoopedStar.CStar.SunlightEnabled( false ); //Disable our star
							Sun.Instance.SunlightEnabled( true ); //Enable Kerbol
							LoopedStar.CStar.Toggle = true;
							
							Planetarium.fetch.Sun = Utils.FindCB( "Sun" ); //Reset this
							foreach( ModuleDeployableSolarPanel panel in FindObjectsOfType( typeof( ModuleDeployableSolarPanel ) ) ) //Reboot solar panels
							{
								panel.OnStart( PartModule.StartState.None );
							}
					}
				}
				else
				{
					//LocalSpace
					//Grab positions
					if( FlightGlobals.ActiveVessel != null )
					{
						dist = FlightGlobals.getAltitudeAtPos(pos2, LoopedStar.CStar.sun);
						if( dist < LoopedStar.CStar.sun.sphereOfInfluence )
						{
							if( LoopedStar.CStar.Toggle == true )
							{
								print ( "Local Enabling Reslah Sun \n");
								LoopedStar.CStar.SunlightEnabled( true );
								Sun.Instance.SunlightEnabled( false );
								
								Planetarium.fetch.Sun = Utils.FindCB( LoopedStar.Name );
							
								foreach( ModuleDeployableSolarPanel panel in FindObjectsOfType( typeof( ModuleDeployableSolarPanel ) ) )
								{
									panel.OnStart( PartModule.StartState.None );
								}
								LoopedStar.CStar.Toggle = false;
							}
						}
						else
						{
							if( LoopedStar.CStar.Toggle == false )
							{
								print ( "Local Disabling Reslah Sun\n" );
								LoopedStar.CStar.SunlightEnabled( false );
								Sun.Instance.SunlightEnabled( true );
								
								Planetarium.fetch.Sun = Utils.FindCB( "Sun" );
							
							
								foreach( ModuleDeployableSolarPanel panel in FindObjectsOfType( typeof( ModuleDeployableSolarPanel ) ) )
								{
									panel.OnStart( PartModule.StartState.None );
								}
								LoopedStar.CStar.Toggle = true;
							}
						}
					}
				}
			}
		}
	}
}

