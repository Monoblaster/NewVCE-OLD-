//---
//	@package VCE
//	@title Package
//	@author Zack0Wack0/www.zack0wack0.com
//	@auther Monoblaster/46426
//	@time 5:30 PM 16/04/2011
//---
package VCE_Main
{
	function fxDtsBrick::onPlant(%brick)
	{
		VCE_createVariableGroup(%brick);
		
		return Parent::onPlant(%brick);
	}
	function fxDtsBrick::onLoadPlant(%brick)
	{
		VCE_createVariableGroup(%brick);
		
		return Parent::onLoadPlant(%brick);
	}
	function gameConnection::ChatMessage(%client,%msg,%client,%brick)
	{
		%obj = %brick;
		if(isObject(%obj))
			return Parent::ChatMessage(%client,%obj.filterVCEString(%msg,%client));
		return Parent::ChatMessage(%client,%msg);
	}
	function gameConnection::CenterPrint(%client,%msg,%time,%client,%brick)
	{
		%obj = %brick;
		if(isObject(%obj))	
			return Parent::CenterPrint(%client,%obj.filterVCEString(%msg,%client), %time);
		return Parent::CenterPrint(%client,%msg,%time);
	}
	function gameConnection::BottomPrint(%client,%msg,%time,%hideBar,%brick)
	{
		%obj = %brick;
		if(isObject(%obj))
			return Parent::BottomPrint(%client,%obj.filterVCEString(%msg,%client), %time, %hideBar);
		return Parent::BottomPrint(%client,%msg,%time, %hideBar);
	}
	function miniGameSO::BottomPrintAll(%mini,%msg,%time,%client,%brick)
	{
		if(isObject(%client))
			%msg = strReplace(%msg,"%1",%client.getPlayerName());
		%obj = %brick;
		if(isObject(%obj)){
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::BottomPrint(%mini.getObject(%i),%obj.filterVCEString(%msg,%mini.getObject(%i)), %time, 0);
		} else{
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::BottomPrint(%mini.getObject(%i),%msg,%time, 0);
		}
	}
	function miniGameSO::CenterPrintAll(%mini,%msg,%time,%client,%brick)
	{
		if(isObject(%client))
			%msg = strReplace(%msg,"%1",%client.getPlayerName());
		%obj = %brick;
		if(isObject(%obj)){
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::CenterPrint(%mini.getObject(%i),%obj.filterVCEString(%msg,%mini.getObject(%i)), %time);
		} else{
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::CenterPrint(%client,%msg,%time);
		}
	}
	function miniGameSO::ChatMsgAll(%mini,%msg,%client,%brick)
	{
		%obj = %brick;
		%obj = 0;
		if(isObject(%obj)){
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::ChatMessage(%mini.getObject(%i),%obj.filterVCEString(%msg,%mini.getObject(%i)));
		} else{
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::ChatMessage(%mini.getObject(%i),%msg);
		}
	}
	function servercmdMessageSent(%client,%message)
	{
		%client.lastMessage = %message;
		%mini = getMinigameFromObject(%client);
		if(isObject(%mini))
			%mini.lastMessage = %message;
		$VCE::Other::LastMessage = %message;
		return Parent::servercmdMessageSent(%client,%message);
	}
	function servercmdteamMessageSent(%client,%message)
	{
		%client.lastTeamMessage = %message;
		return Parent::servercmdteamMessageSent(%client,%message);
	}
	function Armor::onTrigger(%this, %obj, %triggerNum, %val){
		%obj.VCETrigger[%triggerNum] = %val;
		Parent::onTrigger(%this, %obj, %triggerNum, %val);
	}
	function serverCmdSit(%client)
	{
		%client.VCESitting = !%client.player.vceSitting;
		Parent::serverCmdSit(%client);
	}
	function GameConnection::onDeath(%client,%source,%sourceClient,%type,%area)
	{
		%client.vceDeaths++;
		if(%client != %sourceClient && isObject(%sourceClient))
			%sourceClient.vceKills++;
		return Parent::onDeath(%client,%source,%sourceClient,%type,%area);
	}
	function serverCmdAddEvent (%client, %enabled, %inputEventIdx, %delay, %targetIdx, %NTNameIdx, %outputEventIdx, %par1, %par2, %par3, %par4){
		%brick = %client.wrenchBrick;
		%outputName = $OutputEvent_Name[%brick.getClassName(), %outputEventIdx];
		%inputName = $InputEvent_Name[%brick.getClassName(), %inputEventIdx];
		%i = mFloor (%brick.numEvents);
		//startfunction setup
		if(%outputName $= "VCE_StartFunction"){
			%brick.VCE_startFunction(%par1,%par2,%par3,%client)
		}	
		//loop checking
		if((%inputName $= "onVariableTrue" || %inputName $= "onVariableFalse") && (%outputName $= "VCE_ifVariable" || %outputName $= "VCE_ifValue") || (%inputName $= "onVariableUpdate" && (%outputName $= "VCE_ifValue" || %outputName $= "VCE_ifVariable")) || (%inputName $= "onVariableUpdate" && %outputName $= "VCE_modVariable")){
			if (%delay < $Pref::VCE::LoopDelay)
				%delay = $Pref::VCE::LoopDelay;
		}
		Parent::serverCmdAddEvent(%client, %enabled, %inputEventIdx, %delay, %targetIdx, %NTNameIdx, %outputEventIdx, %par1, %par2, %par3, %par4);
	}
	function AIPlayer::startHoleLoop(%bot){
		%bot.hIsEnabled = true;
		Parent::startHoleLoop(%bot);
	}
	function AIPlayer::stopHoleLoop(%bot){
		%bot.hIsEnabled = false;
		Parent::stopHoleLoop(%bot);
	}
	//Repackaging this function to include a brick when giving this to specific output events
	function SimObject::processInputEvent(%obj, %EventName, %client)
	{
		if (%obj.numEvents <= 0.0)
		{
			return;
		}
		%foundOne = 0;
		%i = 0;
		while(%i < %obj.numEvents)
		{
			if (%obj.eventInput[%i] !$= %EventName)
			{
			}
			else
			{
				if (!%obj.eventEnabled[%i])
				{
				}
				else
				{
					%foundOne = 1;
					break;
				}
			}
			%i = %i + 1.0;
		}
		if (!%foundOne)
		{
			return;
		}
		if (isObject(%client))
		{
			%quotaObject = getQuotaObjectFromClient(%client);
		}
		else
		{
			if (%obj.getType() & $TypeMasks::FxBrickAlwaysObjectType)
			{
				%quotaObject = getQuotaObjectFromBrick(%obj);
			}
			else
			{
				if (getBuildString() !$= "Ship")
				{
					error("ERROR: SimObject::ProcessInputEvent() - could not get quota object for event " @ %EventName @ " on object " @ %obj);
				}
				return;
			}
		}
		if (!isObject(%quotaObject))
		{
			error("ERROR: SimObject::ProcessInputEvent() - new quota object creation failed!");
		}
		setCurrentQuotaObject(%quotaObject);
		if (%EventName $= "OnRelay")
		{
			if (%obj.implicitCancelEvents)
			{
				%obj.cancelEvents();
			}
		}
		%i = 0;
		while(%i < %obj.numEvents)
		{
			if (!%obj.eventEnabled[%i])
			{
			}
			else
			{
				if (%obj.eventInput[%i] !$= %EventName)
				{
				}
				else
				{
					if (%obj.eventOutput[%i] !$= "CancelEvents")
					{
					}
					else
					{
						if (%obj.eventDelay[%i] > 0.0)
						{
						}
						else
						{
							if (%obj.eventTarget[%i] == -1.0)
							{
								%name = %obj.eventNT[%i];
								%group = %obj.getGroup();
								%j = 0;
								while(%j < %group.NTObjectCount[%name])
								{
									%target = %group.NTObject[%name,%j];
									if (!isObject(%target))
									{
									}
									else
									{
										%target.cancelEvents();
									}
									%j = %j + 1.0;
								}
							}
							else
							{
								%target = $InputTarget_[%obj.eventTarget[%i]];
								if (!isObject(%target))
								{
								}
								else
								{
									%target.cancelEvents();
								}
							}
						}
					}
				}
			}
			%i = %i + 1.0;
		}
		%eventCount = 0;
		%i = 0;
		while(%i < %obj.numEvents)
		{
			if (%obj.eventInput[%i] !$= %EventName)
			{
			}
			else
			{
				if (!%obj.eventEnabled[%i])
				{
				}
				else
				{
					if (%obj.eventOutput[%i] $= "CancelEvents" && %obj.eventDelay[%i] == 0.0)
					{
					}
					else
					{
						if (%obj.eventTarget[%i] == -1.0)
						{
							%name = %obj.eventNT[%i];
							%group = %obj.getGroup();
							%j = 0;
							while(%j < %group.NTObjectCount[%name])
							{
								%target = %group.NTObject[%name,%j];
								if (!isObject(%target))
								{
								}
								else
								{
									%eventCount = %eventCount + 1.0;
								}
								%j = %j + 1.0;
							}
						}
						else
						{
							%eventCount = %eventCount + 1.0;
						}
					}
				}
			}
			%i = %i + 1.0;
		}
		if (%eventCount == 0.0)
		{
			return;
		}
		%currTime = getSimTime();
		if (%eventCount > %quotaObject.getAllocs_Schedules())
		{
			commandToClient(%client, 'CenterPrint', "<color:FFFFFF>Too many events at once!\n(" @ %EventName @ ")", 1);
			if (%client.SQH_StartTime <= 0.0)
			{
				%client.SQH_StartTime = %currTime;
			}
			else
			{
				if (%currTime - %client.SQH_LastTime < 2000.0)
				{
					%client.SQH_HitCount = %client.SQH_HitCount + 1.0;
				}
				if (%client.SQH_HitCount > 5.0)
				{
					%client.ClearEventSchedules();
					%client.resetVehicles();
					%mask = $TypeMasks::PlayerObjectType | $TypeMasks::ProjectileObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::CorpseObjectType;
					%client.ClearEventObjects(%mask);
				}
			}
			%client.SQH_LastTime = %currTime;
			return;
		}
		if (%currTime - %client.SQH_LastTime > 1000.0)
		{
			%client.SQH_StartTime = 0;
			%client.SQH_HitCount = 0;
		}
		%i = 0;
		while(%i < %obj.numEvents)
		{
			if (%obj.eventInput[%i] !$= %EventName)
			{
			}
			else
			{
				if (!%obj.eventEnabled[%i])
				{
				}
				else
				{
					if (%obj.eventOutput[%i] $= "CancelEvents" && %obj.eventDelay[%i] == 0.0)
					{
					}
					else
					{
						%delay = %obj.eventDelay[%i];
						%outputEvent = %obj.eventOutput[%i];
						%par1 = %obj.eventOutputParameter[%i,1];
						%par2 = %obj.eventOutputParameter[%i,2];
						%par3 = %obj.eventOutputParameter[%i,3];
						%par4 = %obj.eventOutputParameter[%i,4];
						%outputEventIdx = %obj.eventOutputIdx[%i];
						if (%obj.eventTarget[%i] == -1.0)
						{
							%name = %obj.eventNT[%i];
							%group = %obj.getGroup();
							%j = 0;
							while(%j < %group.NTObjectCount[%name])
							{
								%target = %group.NTObject[%name,%j];
								if (!isObject(%target))
								{
								}
								else
								{
									%targetClass = "fxDTSBrick";
									%numParameters = outputEvent_GetNumParametersFromIdx(%targetClass, %outputEventIdx);
									if (%obj.eventOutputAppendClient[%i])
									{
										if (%numParameters == 0.0)
										{
											%scheduleID = %target.schedule(%delay, %outputEvent, %client, %obj);
										}
										else
										{
											if (%numParameters == 1.0)
											{
												%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %client, %obj);
											}
											else
											{
												if (%numParameters == 2.0)
												{
													%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %client, %obj);
												}
												else
												{
													if (%numParameters == 3.0)
													{
														%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %par3, %client, %obj);
													}
													else
													{
														if (%numParameters == 4.0)
														{
															%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %par3, %par4, %client, %obj);
														}
														else
														{
															error("ERROR: SimObject::ProcessInputEvent() - bad number of parameters on event \'" @ %outputEvent @ "\' (" @ numParameters @ ")");
														}
													}
												}
											}
										}
									}
									else
									{
										if (%numParameters == 0.0)
										{
											%scheduleID = %target.schedule(%delay, %outputEvent);
										}
										else
										{
											if (%numParameters == 1.0)
											{
												%scheduleID = %target.schedule(%delay, %outputEvent, %par1);
											}
											else
											{
												if (%numParameters == 2.0)
												{
													%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2);
												}
												else
												{
													if (%numParameters == 3.0)
													{
														%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %par3);
													}
													else
													{
														if (%numParameters == 4.0)
														{
															%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %par3, %par4);
														}
														else
														{
															error("ERROR: SimObject::ProcessInputEvent() - bad number of parameters on event \'" @ %outputEvent @ "\' (" @ numParameters @ ")");
														}
													}
												}
											}
										}
									}
									if (%delay > 0.0)
									{
										%obj.addScheduledEvent(%scheduleID);
									}
								}
								%j = %j + 1.0;
							}
						}
						else
						{
							%target = $InputTarget_[%obj.eventTarget[%i]];
							if (!isObject(%target))
							{
							}
							else
							{
								%targetClass = inputEvent_GetTargetClass("fxDTSBrick", %obj.eventInputIdx[%i], %obj.eventTargetIdx[%i]);
								%numParameters = outputEvent_GetNumParametersFromIdx(%targetClass, %outputEventIdx);
								if (%obj.eventOutputAppendClient[%i])
								{
									if (%numParameters == 0.0)
									{
										%scheduleID = %target.schedule(%delay, %outputEvent, %client,%obj);
									}
									else
									{
										if (%numParameters == 1.0)
										{
											%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %client,%obj);
										}
										else
										{
											if (%numParameters == 2.0)
											{
												%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %client,%obj);
											}
											else
											{
												if (%numParameters == 3.0)
												{
													%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %par3, %client,%obj);
												}
												else
												{
													if (%numParameters == 4.0)
													{
														%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %par3, %par4, %client,%obj);
													}
													else
													{
														error("ERROR: SimObject::ProcessInputEvent() - bad number of parameters on event \'" @ %outputEvent @ "\' (" @ numParameters @ ")");
													}
												}
											}
										}
									}
								}
								else
								{
									if (%numParameters == 0.0)
									{
										%scheduleID = %target.schedule(%delay, %outputEvent);
									}
									else
									{
										if (%numParameters == 1.0)
										{
											%scheduleID = %target.schedule(%delay, %outputEvent, %par1);
										}
										else
										{
											if (%numParameters == 2.0)
											{
												%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2);
											}
											else
											{
												if (%numParameters == 3.0)
												{
													%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %par3);
												}
												else
												{
													if (%numParameters == 4.0)
													{
														%scheduleID = %target.schedule(%delay, %outputEvent, %par1, %par2, %par3, %par4);
													}
													else
													{
														error("ERROR: SimObject::ProcessInputEvent() - bad number of parameters on event \'" @ %outputEvent @ "\' (" @ numParameters @ ")");
													}
												}
											}
										}
									}
								}
								if (%delay > 0.0 && %EventName !$= "onToolBreak")
								{
									%obj.addScheduledEvent(%scheduleID);
								}
							}
						}
					}
				}
			}
			%i = %i + 1.0;
		}
	}

};
