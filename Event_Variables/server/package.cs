//---
//	@package VCE
//	@title Package
//	@author Zack0Wack0/www.zack0wack0.com
//	@auther Monoblaster/46426
//	@time 5:30 PM 16/04/2011
//---
$VCEisEventParameterType["int"] = 1;
$VCEisEventParameterType["float"] = 1;
$VCEisEventParameterType["list"] = 1;
$VCEisEventParameterType["bool"] = 1;
$VCEisEventParameterType["intList"] = 1;
$VCEisEventParameterType["datablock"] = 1;
$VCEisEventParameterType["string"] = 1;
$VCEisEventParameterType["vector"] = 1;
$VCEisEventParameterType["paintColor"] = 1;
//MIM between proccessing and actual event calling
function SimObject::VCECallEvent(%obj, %outputEvent, %brick, %client, %passClient, %par1, %par2, %par3, %par4)
{
	talk(%client.player SPC %outputevent);
	%classname = %obj.getClassName();

	%parameterWords = verifyOutputParameterList(%classname, outputEvent_GetOutputEventIdx(%classname, %outputEvent));
	%parameterWordCount = getWordCount(%parameterWords);
	
	%c = 1;
	//filter all string parameters
	for(%i = 0; %i < %parameterWordCount; %i++)
	{
		%word = getWord(%parameterWords, %i);
		
		if(%word $= "string")
			%par[%c] = %brick.filterVCEString(%par[%c],%client);

		if($VCEisEventParameterType[%word])
			%c++;

	}

	%parCount = outputEvent_GetNumParametersFromIdx(%classname, outputEvent_GetOutputEventIdx(%classname, %outputEvent));

	//call the event correctly with the right number of paramter
	//for some reason some events have error detection for the wrong number of paramters
	//we could use eval but i'd rather not be compiling code during runtime

	if(%passClient)
	{
		if(%parCount == 0)
			%obj.call(%outputEvent,%client);
		if(%parCount == 1)
			%obj.call(%outputEvent,%par1,%client);
		if(%parCount == 2)
			%obj.call(%outputEvent,%par1,%par2,%client);
		if(%parCount == 3)
			%obj.call(%outputEvent,%par1,%par2,%par3,%client);
		if(%parCount == 4)
			%obj.call(%outputEvent,%par1,%par2,%par3,%par4,%client);	
	}
	else
	{
		if(%parCount == 0)
			%obj.call(%outputEvent);
		if(%parCount == 1)
			%obj.call(%outputEvent,%par1);
		if(%parCount == 2)
			%obj.call(%outputEvent,%par1,%par2);
		if(%parCount == 3)
			%obj.call(%outputEvent,%par1,%par2,%par3);
		if(%parCount == 4)
			%obj.call(%outputEvent,%par1,%par2,%par3,%par4);	
	}
}
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
			%brick.VCE_startFunction(%par1,%par2,%par3,%client);
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
									if (%numParameters == 0.0)
									{
										%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i]);
									}
									else
									{
										if (%numParameters == 1.0)
										{
											%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i], %par1);
										}
										else
										{
											if (%numParameters == 2.0)
											{
												%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i], %par1, %par2);
											}
											else
											{
												if (%numParameters == 3.0)
												{
													%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i], %par1, %par2, %par3);
												}
												else
												{
													if (%numParameters == 4.0)
													{
														%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i], %par1, %par2, %par3, %par4);
													}
													else
													{
														error("ERROR: SimObject::ProcessInputEvent() - bad number of parameters on event \'" @ %outputEvent @ "\' (" @ numParameters @ ")");
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
								if (%numParameters == 0.0)
								{
									%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i]);
								}
								else
								{
									if (%numParameters == 1.0)
									{
										%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i], %par1);
									}
									else
									{
										if (%numParameters == 2.0)
										{
											%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i], %par1, %par2);
										}
										else
										{
											if (%numParameters == 3.0)
											{
												%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i], %par1, %par2, %par3);
											}
											else
											{
												if (%numParameters == 4.0)
												{
													%scheduleID = %target.schedule(%delay,"VCECallEvent", %outputEvent, %obj, %client, %obj.eventOutputAppendClient[%i], %par1, %par2, %par3, %par4);
												}
												else
												{
													error("ERROR: SimObject::ProcessInputEvent() - bad number of parameters on event \'" @ %outputEvent @ "\' (" @ numParameters @ ")");
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
