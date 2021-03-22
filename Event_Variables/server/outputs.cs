//---
//	@package VCE
//	@title Output Events
//	@author Zack0Wack0/www.zack0wack0.com
//	@auther Monoblaster/46426
//	@time 4:39 PM 15/03/2011
//---
$VCE::Server::ImmuneTime = 5000;
function fxDTSBrick::doVCEVarFunction(%brick, %function, %oldValue, %newValue, %client){
	%newValue = strReplace(%newValue, ",", "\t");
	if(%function == 0)
		return getSubStr(%newValue,0,32768); //we do nothing as it's done already + substring to prevent overflows
	if(%function == 1)
		return %oldvalue + getField(%newvalue, 0);
	if(%function == 2)
		return %oldvalue - getField(%newvalue, 0);
	if(%function == 3)
		return %oldvalue * getField(%newvalue, 0);
	if(%function == 4)
		return %oldvalue / getField(%newvalue, 0);
	if(%function == 15)
		return %oldvalue % getField(%newvalue, 0);
	if(%function == 7)
		return mPow(%oldValue, getField(%newvalue, 0));
	if(%function == 8)
		return mPow(%oldValue, 1 / getField(%newvalue, 0));
	if(%function == 9)
		return mPercent(%oldvalue, getField(%newvalue, 0));
	if(%function == 10)
		return getRandom(%oldValue, getField(%newvalue, 0));
	if(%function == 17)
		return mAbs(%oldValue);
	if(%function == 5)
		return mFloor(%oldValue);
	if(%function == 6)
		return mCeil(%oldValue);
	if(%function == 18)
		return mClamp(%oldValue, getField(%newValue, 0), getField(%newValue, 1));
	if(%function == 19)
		return mSin(%oldValue);
	if(%function == 20)
		return mCos(%oldValue);
	if(%function == 21)
		return mTan(%oldValue);
	if(%function == 22)
		return mASin(%oldValue);
	if(%function == 23)
		return mACos(%oldValue);
	if(%function == 24)
		return mATan(%oldValue);
	if(%function == 15)
		return strLen(%oldValue);
	if(%function == 25)
		return strPos(%oldValue, getField(%newValue, 0), getField(%newValue, 1));
	if(%function == 12)
		return strLwr(%oldValue);
	if(%function == 13)
		return strUpr(%oldValue);
	if(%function == 14) 
		return strChr(%oldValue, getField(%newValue, 0));
	if(%function == 26)
		return strReplace(%oldValue, getField(%newValue, 0), getField(%newValue, 1));
	if(%function == 27)
		return trim(%oldValue);
	if(%function == 28)
		return getSubStr(%oldValue, getField(%newValue, 0), getField(%newValue, 1));
	if(%function == 11)
		return getWord(%oldValue, getField(%newValue, 0));
	if(%function == 29)
		return getWordCount(%oldValue);
	if(%function == 30)
		return getWords(%oldValue, getField(%newValue, 0), getField(%newValue, 1));
	if(%function == 31)
		return removeWord(%oldValue, getField(%newValue, 0));
	if(%function == 32)
		return removeWords(%oldValue,getField(%newValue, 0),getField(%newValue, 0));
	if(%function == 33)
		return setWord(%oldValue, getField(%newValue, 0), getField(%newValue, 1));
	if(%function == 34)
		return vectorDist(%oldValue, getField(%newValue, 0));
	if(%function == 35)
		return vectorAdd(%oldValue, getField(%newValue, 0));
	if(%function == 36)
		return vectorSub(%oldValue, getField(%newValue, 0));
	if(%function == 37)
		return vectorScale(%oldValue, getField(%newValue, 0));
	if(%function == 38)
		return vectorLen(%oldValue);
	if(%function == 39)
		return vectorNormalize(%oldValue);
	if(%function == 40)
		return vectorDot(%oldValue, getField(%newValue, 0));
	if(%function == 41)
		return vectorCross(%oldValue, getField(%newValue, 0));
	if(%function == 42)
		return getBoxCenter(%oldValue);
	if(%function == 43)
		return %oldValue && getField(%newValue, 0);
	if(%function == 44)
		return %oldValue || getField(%newValue, 0);
	if(%function == 45)
		return %oldValue & getField(%newValue, 0);
	if(%function == 46)
		return %oldValue | getField(%newValue, 0);
	if(%function == 47)
		return %oldValue >> getField(%newValue, 0);
	if(%function == 48)
		return %oldValue << getField(%newValue, 0);
	if(%function == 49)
		return %oldValue ^ getField(%newValue, 0);
	if(%function == 50)
		return ~%oldValue;
	if(%function == 51)
		return !%oldValue;
	if(%function == 52)
		return %oldValue $= getField(%newValue, 0);
	if(%function == 53)
		return %oldValue !$= getField(%newValue, 0);
	if(%function == 54)
		return %oldValue > getField(%newValue, 0);
	if(%function == 55)
		return %oldValue < getField(%newValue, 0);
	if(%function == 56)
		return %oldValue >= getField(%newValue, 0);
	if(%function == 57)
		return %oldValue <= getField(%newValue, 0);
	if(%function == 58)
		return strPos(%oldValue,getField(%newValue, 0)) > -1;
}
function SimObject::VCE_modVariable(%obj,%varName,%logic,%value,%client,%brick){
	if(!(isObject(%client) || isObject(%obj) || isObject(%brick)))
		return;
	%className = %obj.getClassName(); 
	%varName = %brick.filterVCEString(%varName,%client);
	%oldvalue = %brick.getGroup().vargroup.getVariable(%varName,%obj);
	%newvalue = %brick.filterVCEString(%value,%client);

	if($VCE::Server::SpecialVarEdit[%className,%varName] !$= ""){
		if($Pref::VCE::canEditSpecialVars)
			%oldvalue = eval("return" SPC strreplace($VCE::Server::SpecialVar[%className,%varName],"%this",%obj) @ ";");
		else
			return;
	}

	%newValue = %brick.doVCEVarFunction(%logic, %oldValue, %newValue,%client);
	%f = "VCE_" @ $VCE::Server::ObjectToReplacer[%className] @ "_" @ $VCE::Server::SpecialVarEdit[%className,%varName];

	if(isFunction(%f))
		call(%f,%obj,%newvalue,$VCE::Server::SpecialVarEditArg1[%className,%varName],$VCE::Server::SpecialVarEditArg2[%className,%varName],$VCE::Server::SpecialVarEditArg3[%className,%varName],$VCE::Server::SpecialVarEditArg4[%className,%varName]);
	else
		%brick.getGroup().vargroup.setVariable(%varName,%newValue,%obj);

	%brick.onVariableUpdate(%client);
}

package VCE_modVariable{
	function fxDtsBrick::VCE_modVariable(%brick,%type,%varName,%logic,%value,%client)
	{
		if(isObject( %obj = VCE_getObjectFromVarType(%type,%brick,%client))){
			Parent::VCE_modVariable(%obj, %varName, %logic, %value, %client, %brick);
		}
	}
};
function SimObject::VCE_ifVariable(%obj,%var,%logic,%valb,%subdata,%client,%brick){
	%var = %brick.filterVCEString(%var,%client);
	for(%i = 0; %i < getWordCount(%var); %i++){
		if((%value = %brick.getGroup().vargroup.getVariable(getWord(%var ,%i), %obj)) !$= ""){
			%var = setWord(%var, %i, %value);
		}
	}
	%brick.VCE_ifValue(%var, %logic, %valb, %subdata, %client);
}
function fxDtsBrick::VCE_ifValue(%brick,%vala,%logic,%valb,%subdata,%client)
{
	if(!isObject(%client))
		return;
	if(isObject(%brick.getGroup().vargroup))
	{
		%vala = %brick.filterVCEString(%vala,%client);
		%valb = %brick.filterVCEString(%valb,%client);
		%test = %brick.doVCEVarFunction(%logic + 52,%vala,%valb,%client);

		%subStart = getWord(%subData,0);
		%subEnd = getWord(%subData,1);

		if(%subStart $= "")
			%subStart = -1;
		if(%subEnd $= "")	
			%subEnd =  -1;

		if(%test)
			%brick.VCE_ProcessVCERange(%subStart, %subEnd, "onVariableTrue", %client);
		else
		    %brick.VCE_ProcessVCERange(%subStart, %subEnd, "onVariableFalse", %client);
	}
}
function fxDtsBrick::VCE_retroCheck(%brick,%vala,%logic,%valb,%subdata,%client)
{
	if(isObject(%brick.getGroup().vargroup))
	{
		//ifPlayerName 0 ifPlayerID 1 ifAdmin 2 ifPlayerEnergy 3 ifPlayerDamage 4 ifPlayerScore 5 ifLastPlayerMsg 6 ifBrickName 7 ifRandomDice 8
		%valb = %brick.filterVCEString(%valb,%client);
		if(%vala == 0)
			%vala = %client.name;
		else if(%vala == 1)
			%vala = %client.BL_ID;
		else if(%vala == 2){
			%vala = %client.isAdmin;
			%valb = %client.isAdmin == 1 ? 1 : -1;
		} else if(%vala == 3){
			%vala = 0;
			if(isObject(%client.player))
				%vala = %client.player.getEnergyLevel();
		} else if(%vala == 4){
			%vala = 0;
			if(isObject(%client.player))
				%vala = %client.player.getDamageLevel();
		} else if(%vala == 5)
			%vala = %client.score;
		else if(%vala == 6)
			%vala = %client.lastMessage;
		else if(%vala == 7){
			if(strLen(%brick.getName()) >= 1)
				%vala = getSubStr(%brick.getName(),1,strLen(%brick.getName()) - 1);
		} else if(%vala == 8)
			%vala = getRandom(1,6);
		%brick.VCE_ifValue(%vala,%logic,%valb,%subdata,%client);
	}
}
function fxDtsBrick::VCE_stateFunction(%brick,%name,%subdata,%client)
{
	%brick.VCE_StartFunction(0,%name,%subdata,%client);
}
function fxDTSBrick::VCE_startFunction(%brick,%type,%name,%subData,%client)
{

	if(!isObject(%varGroup = %brick.getGroup().vargroup))
		return;
	%name = %brick.filterVCEString(%name,%client);

	%subStart = getWord(%subData,0);
	%subEnd = getWord(%subData,1);

	if(%subStart $= "")
		%subStart = -1;
	if(%subEnd $= "")	
		%subEnd =  -1;

	if(%type == 0)
		%brick.vceFunction[%name] = %subStart SPC %subEnd;

	if(%type == 1)
	{
		if((%c = %varGroup.GetLocalFunctionFromBrick(%name,%brick)) > 0)
			%varGroup.vceLocalFunction[%name,%c] = %brick SPC %substart SPC %subEnd;
		else
			%varGroup.vceLocalFunction[%name,%varGroup.vceLocalFunctionCount[%name]++] = %brick SPC %substart SPC %subEnd;
	}

}
function VariableGroup::GetLocalFunctionFromBrick(%varGroup,%name,%brick)
{
	if(!isObject(%brick))
		return;
	%total = %varGroup.vceLocalFunctionCount[%name];
	%c = 1;
	while(%c <= %total && getWord(%varGroup.vceLocalFunction[%name,%c], 0) != %brick)
		%c++;
	if(%c > %total)
		return 0;
	return %c;
}
function SimObject::VCE_callFunction(%obj,%name,%args,%delay,%client,%brick)
{
	if(!isObject(%client))
		return;
	if(isObject(%obj.getGroup().vargroup))
	{
		%varGroup = %obj.getGroup().vargroup;
		
		%name = %brick.filterVCEString(%name,%client);
		%delay = %brick.filterVCEString(%delay,%client);
		%args = %brick.filterVCEString(%args,%client);

		if(%delay < 0)
			%delay = 0;
		%args = strReplace(%args,"|","\t");
		%args = strReplace(%args,",","\t");
		%fc = getFieldCount(%args);
		if(%obj.vceFunction[%name] !$= "")
		{
			for(%i=0;%i<%fc;%i++)
			{
				%arg[%i] = getField(%args,%i);		
				%varGroup.setVariable("arg" @ %i,%arg[%i],%obj);
			}	

			%subStart = getWord(%obj.vceFunction[%name], 0);
			%subEnd = getWord(%obj.vceFunction[%name], 1);

			%varGroup.setVariable("argcount",getFieldCount(%args),%obj);

			%obj.VCE_ProcessVCERange(%subStart, %subEnd, "onVariableFunction", %client);
		} 
		else if((%count = %vargroup.vceLocalFunctionCount[%name]) > 0)
		{
			for(%i = 1; %i <= %count; %i++)
			{
				%sentence = %vargroup.vceLocalFunction[%name,%i];
				
				%localbrick = getWord(%sentence,0);
				%subStart = getWord(%sentence,1);
				%subEnd = getWord(%sentence,0);

				if(!isObject(%localbrick))
					continue;
				
				for(%j=0;%j<%fc;%j++)
				{
					%arg[%j] = getField(%args,%j);		
					%varGroup.setVariable("arg" @ %j,%arg[%j],%localBrick);
				}

				%varGroup.setVariable("argcount",getFieldCount(%args),%localBrick);

				%localbrick.VCE_ProcessVCERange(%subStart, %subEnd, "onVariableFunction", %client);
			}
		}
		
	}
}
function fxDTSBrick::VCE_cancelFunction(%brick,%name,%client){
	%name = %brick.filterVariableString(%name,%client);
	%count = %brick.functionScheduleCount[%name];
	for(%i = 0; %i < %count; %i++)
	%schedule = %brick.functionSchedule[%i,%name];
		if(isEventPending(%schedule))
			cancel(%schedule);
	%brick.functionScheduleCount[%name] = 0;
}
function fxDtsBrick::VCE_relayCallFunction(%brick,%direction,%name,%args,%delay,%client)
{
	%name = %brick.filterVCEString(%name,%client);
	%args = %brick.filterVCEString(%args,%client);
	%delay = %brick.filterVCEString(%delay,%client);
	%WB = %brick.getWorldBox ();
	%sizeX = (getWord (%WB, 3) - getWord (%WB, 0)) - 0.1;
	%sizeY = (getWord (%WB, 4) - getWord (%WB, 1)) - 0.1;
	%sizeZ = (getWord (%WB, 5) - getWord (%WB, 2)) - 0.1;
	%pos = %brick.getPosition ();
	%posX = getWord (%pos, 0);
	%posY = getWord (%pos, 1);
	%posZ = getWord (%pos, 2);
	if(%direction == 0){
		%posZ = getWord (%pos, 2) + %sizeZ / 2 + 0.05;
		%size = %sizeX SPC %sizeY SPC 0.1;
	} else if(%direction == 1){
		%posZ = (getWord (%pos, 2) - %sizeZ / 2) - 0.05;
		%size = %sizeX SPC %sizeY SPC 0.1;
	} else if(%direction == 2){
		%posY = getWord (%pos, 1) + %sizeY / 2 + 0.05;
		%size = %sizeX SPC 0.1 SPC %sizeZ;
	} else if(%direction == 3){
		%posX = getWord (%pos, 0) + %sizeX / 2 + 0.05;
		%size = 0.1 SPC %sizeY SPC %sizeZ;
	} else if(%direction == 4){
		%posY = (getWord (%pos, 1) - %sizeY / 2) - 0.05;
		%size = %sizeX SPC 0.1 SPC %sizeZ;
	} else if(%direction == 5){
		%posX = (getWord (%pos, 0) - %sizeX / 2) - 0.05;
		%size = 0.1 SPC %sizeY SPC %sizeZ;
	}
	%pos = %posX SPC %posY SPC %posZ;
	%mask = $TypeMasks::FxBrickAlwaysObjectType;
	%group = %brick.getGroup ();
	initContainerBoxSearch (%pos, %size, %mask);
	while ((%searchObj = containerSearchNext ()) != 0)
	{
		if (!%searchObj.getGroup () == %group)
		{
			
		}
		else if (%searchObj == %brick)
		{
			
		}
		else if (%searchObj.numEvents <= 0)
		{
			
		}
		else 
		{
			%searchObj.VCE_callFunction(%name,%args,%delay,%client,%brick);
		}
	}
}
function fxDtsBrick::VCE_saveVariable(%brick,%type,%vars,%client)
{
	%varGroup = %brick.getGroup().vargroup;
	if(!(isObject(%client) || isObject(%varGroup)))
		return;

	%obj = VCE_getObjectFromVarType(%type,%brick,%client);

	%vars = strReplace(%vars,",","\t");
	%count = getFieldCount(%vars);
	for(%i=0;%i<%count;%i++)
		%vargroup.saveVariable(trim(getField(%vars,%i)),%obj);

}
function fxDtsBrick::VCE_loadVariable(%brick,%type,%vars,%client)
{
	if(!isObject(%client))
		return;
	if(isObject(%brick.getGroup().vargroup))
	{
		%obj = VCE_getObjectFromVarType(%type,%brick,%client);

		if(!isObject(%obj))
			return;
		%vargroup = %brick.getGroup().vargroup;
		%vars = strReplace(%vars,",","\t");
		%count = getFieldCount(%vars);
		for(%i=0;%i<%count;%i++)
			%vargroup.loadVariable(trim(getField(%vars,%i)),%obj);
	}
}

//Stolen from firerelaynum as this is the best and strongest solution
function SimObject::VCE_ProcessVCERange(%obj, %start, %end, %inputEvent, %client)
{
	// Only check for those events we are interested in
	if (%start < 0 || %end > %obj.numevents || %start > %end){
		%start = 0;
		%end = %obj.numevents;
	}
	for (%i = %start; %i <= %end; %i++)
	{
		
		// Already processed
		if (%tempEvent[%i])
			continue;

		// Enabled event
		if (!%obj.eventEnabled[%i])
			continue;
		
		// Not onRelay
		if (%obj.eventInput[%i] !$= %inputEvent)
			continue;
		
		// Target brick(s)
		if (%obj.eventTargetIdx[%i] == -1)
		{
			%type = "fxDTSBrick";
			%group = getBrickGroupFromObject(%obj);
			%name = %obj.eventNT[%i];
			for (%objs = 0; %objs < %group.NTObjectCount[%name]; %objs++)
				%objs[%objs] = %group.NTObject[%name, %objs];
		}
		// Self
		else
		{
			%type = inputEvent_GetTargetClass(%obj.getClassName(), %obj.eventInputIdx[%i], %obj.eventTargetIdx[%i]);
			%objs = 1;
			// Get object from type (Event_onRelay)
			switch$ (%type)
			{
			case "Bot":
				%objs0 = %obj.hBot;
			case "Player":
				%objs0 = %client.player;
			case "GameConnection":
				%objs0 = %client;
			case "Minigame":
				%objs0 = getMinigameFromObject(%client);
			default:
				%objs0 = %obj;
			}
		}

		// Parameters
		%numParams = outputEvent_GetNumParametersFromIdx(%type, %obj.eventOutputIdx[%i]);
		
		// Get parameters
		%param = "";
		for (%n = 1; %n <= %numParams; %n++)
			%p[%n] = %obj.eventOutputParameter[%i, %n];
		
		// Append client
		if (%obj.eventOutputAppendClient[%i] && isObject(%client))
		{
			%p[%n] = %client;
			%numParams++;
		}

		%eventDelay = %obj.eventDelay[%i];
		%eventOutput = %obj.eventOutput[%i];
		
		// Go through list/brick
		for (%n = 0; %n < %objs; %n++)
		{
			%next = %objs[%n];

			if (!isObject(%next))
				continue;
			
			// Call for event function
			switch (%numParams)
			{
			case 0: %event = %next.schedule(%eventDelay, %eventOutput,%obj);
			case 1: %event = %next.schedule(%eventDelay, %eventOutput, %p1,%obj);
			case 2: %event = %next.schedule(%eventDelay, %eventOutput, %p1, %p2,%obj);
			case 3: %event = %next.schedule(%eventDelay, %eventOutput, %p1, %p2, %p3,%obj);
			case 4: %event = %next.schedule(%eventDelay, %eventOutput, %p1, %p2, %p3, %p4,%obj);
			case 5: %event = %next.schedule(%eventDelay, %eventOutput, %p1, %p2, %p3, %p4, %p5,%obj);
			}
			
			// To be able to cancel an event
			if (%delay > 0)
				%obj.addScheduledEvent(%event);
		}

		// Mark as processed
		%tempEvent[%i] = 1;
	}
	return "";
}