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
function fxDtsBrick::VCE_modVariable(%brick,%type,%name,%logic,%value,%client)
{
	%client.eventProcessingObj = %brick;
	if(!isObject(%client))
		return;
	%varType = $VCE::Sever::VarType[%type];
	%category = getField(%varType,0);
	%target = eval("return" SPC getField(%varType,2) @ "\;");
	%class = getField(%varType,1);
	if(!isObject(%target))
		return;
	%name = %brick.filterVCEString(%name,%client);
	%oldvalue = %brick.getGroup().vargroup.getVariable(%category,%name,%target);
	%newvalue = %brick.filterVCEString(%value,%client);
	if($VCE::Server::SpecialVarEdit[%class,%name] !$= "" && isObject(%target)){
		if($Pref::VCE::canEditSpecialVars)
			%oldvalue = eval("return" SPC strreplace($VCE::Server::SpecialVar[%class,%var],"%this",%target) @ ";");
		else
			return;
	}
	%newValue = %brick.doVCEVarFunction(%logic, %oldValue, %newValue,%client);
	%f = "VCE_" @ %category @ "_" @ $VCE::Server::SpecialVarEdit[%class,%name];
	if(isFunction(%f))
	{
		call(%f,%target,%newvalue,$VCE::Server::SpecialVarEditArg1[%class,%name],$VCE::Server::SpecialVarEditArg2[%class,%name],$VCE::Server::SpecialVarEditArg3[%class,%name],$VCE::Server::SpecialVarEditArg4[%class,%name]);
		return;
	} else{
		%brick.getGroup().vargroup.setVariable(%category,%name,%newValue,%target);
	}
	%brick.onVariableUpdate(%client);
}
function GameConnection::VCE_modVariable(%client,%name,%logic,%value,%client)
{
	$inputTarget_Self.VCE_modVariable(2, %name, %logic, %value, %client);
}
function Player::VCE_modVariable(%player,%name,%logic,%value,%client)
{
	$inputTarget_Self.VCE_modVariable(1, %name, %logic, %value, %client);
}
function Vehicle::VCE_modVariable(%vehicle,%name,%logic,%value,%client)
{
	$inputTarget_Self.VCE_modVariable(4, %name, %logic, %value, %client);
}
function AIPlayer::VCE_modVariable(%bot,%name,%logic,%value,%client)
{
	if($inputTarget_Self.getDataBlock().getName() $= "brickVehicleSpawnData")
		$inputTarget_Self.VCE_modVariable(4, %name, %logic, %value, %client);
	else
		$inputTarget_Self.VCE_modVariable(5, %name, %logic, %value, %client);
}
function MinigameSO::VCE_modVariable(%mini,%name,%logic,%value,%client)
{
	$inputTarget_Self.VCE_modVariable(3, %name, %logic, %value, %client);
}
function MinigameSO::VCE_ifVariable(%mini,%var,%logic,%valb,%subdata,%client)
{
	%var = $inputTarget_Self.filterVCEString(%var,%client);
	for(%i = 0; %i < getWordCount(%var); %i++){
		if((%value = $inputTarget_Self.getGroup().vargroup.getVariable("MinigameSO",getWord(%var ,%i),%mini)) !$= ""){
			%var = setWord(%var, %i, %value);
		}
	}
	$inputTarget_Self.VCE_ifValue(%var, %logic, %valb, %subdata, %client);
}
function Vehicle::VCE_ifVariable(%vehicle,%var,%logic,%valb,%subdata,%client)
{
	%var = $inputTarget_Self.filterVCEString(%var,%client);
	for(%i = 0; %i < getWordCount(%var); %i++){
		if((%value = $inputTarget_Self.getGroup().vargroup.getVariable("Vehicle",getWord(%var ,%i),%vehicle)) !$= ""){
			%var = setWord(%var, %i, %value);
		}
	}
	$inputTarget_Self.VCE_ifValue(%var, %logic, %valb, %subdata, %client);
}
function Player::VCE_ifVariable(%player,%var,%logic,%valb,%subdata,%client)
{
	%var = $inputTarget_Self.filterVCEString(%var,%client);
	for(%i = 0; %i < getWordCount(%var); %i++){
		if((%value = $inputTarget_Self.getGroup().vargroup.getVariable("Player",getWord(%var ,%i),%player)) !$= ""){
			%var = setWord(%var, %i, %value);
		}
	}
	$inputTarget_Self.VCE_ifValue(%var, %logic, %valb, %subdata, %client);
}
function GameConnection::VCE_ifVariable(%client,%var,%logic,%valb,%subdata,%sourceClient)
{
	%var = $inputTarget_Self.filterVCEString(%var,%client);
	for(%i = 0; %i < getWordCount(%var); %i++){
		if((%value = $inputTarget_Self.getGroup().vargroup.getVariable("GameConnection",getWord(%var ,%i),%client)) !$= ""){
			%var = setWord(%var, %i, %value);
		}
	}
	$inputTarget_Self.VCE_ifValue(%var, %logic, %valb, %subdata, %client);
}
function fxDtsBrick::VCE_ifVariable(%brick,%var,%logic,%valb,%subdata,%client)
{
	%var = %brick.filterVCEString(%var,%client);
	for(%i = 0; %i < getWordCount(%var); %i++){
		if((%value = $inputTarget_Self.getGroup().vargroup.getVariable("Brick",getWord(%var ,%i),%brick)) !$= ""){
			%var = setWord(%var, %i, %value);
		}
	}
	%brick.VCE_ifValue(%var, %logic, %valb, %subdata, %client);
}
function AIPlayer::VCE_ifVariable(%bot,%var,%logic,%valb,%subdata,%client)
{
	%var = $inputTarget_Self.filterVCEString(%var,%client);
	for(%i = 0; %i < getWordCount(%var); %i++){
		if((%value = $inputTarget_Self.getGroup().vargroup.getVariable("AIPlayer",getWord(%var ,%i),%bot)) !$= ""){
			%var = setWord(%var, %i, %value);
		}
	}
	$inputTarget_Self.VCE_ifValue(%var, %logic, %valb, %subdata, %client);
}
function fxDtsBrick::VCE_ifValue(%brick,%vala,%logic,%valb,%subdata,%client)
{
	%client.eventProcessingObj = %brick;
	if(!isObject(%client))
		return;
	if(isObject(%brick.getGroup().vargroup))
	{
		%vala = %brick.filterVCEString(%vala,%client);
		%valb = %brick.filterVCEString(%valb,%client);
		%test = %brick.doVCEVarFunction(%logic + 52,%vala,%valb,%client);
		%subStart = mClamp(getWord(%subdata,0),0,%brick.numEvents);
		%subEnd = mClamp(getWord(%subdata,1),0,%brick.numEvents);
		if(%subStart == 0 && %subEnd == 0){
			%subStart = 0;
			%subEnd = %brick.numEvents - 1;
		}
		if(%test)
			%brick.onvariableTrue(%client,%subStart, %subEnd);
		else
			%brick.onvariableFalse(%client,%subStart, %subEnd);
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
	%client.eventProcessingObj = %brick;
	if(isObject(%brick.getGroup().vargroup))
	{
		if(getWordCount(%subdata) != 2)
			return;
		%name = %brick.filterVCEString(%name,%client);
		%subStart = mClamp(getWord(%subdata,0),0,%brick.numEvents);
		%subEnd = mClamp(getWord(%subdata,1),0,%brick.numEvents);
		if(%subStart == 0 && %subEnd == 0){
			%subStart = 0;
			%subEnd = %brick.numEvents - 1;
		}
		%brick.vceFunction[%name] = %substart SPC %subend;
	}
}
function fxDtsBrick::VCE_callFunction(%brick,%name,%args,%delay,%client)
{
	%preBrick = %client.eventProcessingObj;
	if(!isObject(%client))
		return;
	%client.eventProcessingObj = %brick;
	if(isObject(%brick.getGroup().vargroup))
	{
		if(%brick == %preBrick){
			%name = %brick.filterVCEString(%name,%client);
			%delay = %brick.filterVCEString(%delay,%client);
			%args = %brick.filterVCEString(%args,%client);
		} else{
			%name = %preBrick.filterVCEString(%name,%client);
			%delay = %preBrick.filterVCEString(%delay,%client);
			%args = %preBrick.filterVCEString(%args,%client);
		}
		if(%delay < 0)
			%delay = 0;
		%args = strReplace(%args,"|","\t");
		%args = strReplace(%args,",","\t");
		%fc = getFieldCount(%args);
		for(%i=0;%i<%fc;%i++)
		{
			%arg[%i] = getField(%args,%i);		
			%brick.getGroup().vargroup.setVariable("Brick","arg" @ %i,%arg[%i],%brick);
		}
		if(getWordCount(%brick.vceFunction[%name]) < 2 && !%subStart)
		{	
			%brick.onVariableFunction(%client,0,%brick.numEvents);
			%client.eventProcessingObj = %preBrick;
			return;
		}
		if(!%subStart){
			%subStart = getWord(%brick.vceFunction[%name], 0);
			%subEnd = getWord(%brick.vceFunction[%name], 1);
		}
		%brick.getGroup().vargroup.setVariable("Brick","argcount",getFieldCount(%args),%brick);
		if(%subStart == 0 && %subEnd == 0){
			%subStart = 0;
			%subEnd = %brick.numEvents - 1;
		}
		%schedule = %brick.schedule(%delay, onVariableFunction, %client, %subStart, %subEnd);
		if(%brick.functionScheduleCount[%name] $= "")
			%brick.functionScheduleCount[%name] = 0;
		%brick.functionSchedule[%brick.functionScheduleCount[%name],%name] = %schedule;
		%brick.functionScheduleCount[%name]++;
		%brick.addScheduledEvent(%brick, %schedule);
	}
	%client.eventProcessingObj = %preBrick;
}
function fxDTSBrick::VCE_cancelFunction(%brick,%name,%client){
	%client.eventProcessingObj = %brick;
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
			%searchObj.VCE_callFunction(%name,%args,%delay,%client);
		}
	}
	%client.eventProcessingObj = %brick;
}
function fxDTSBrick::VCE_startFunction(%brick,%name,%range,%client){
	//empty function as it's just a pointer
}
function fxDtsBrick::VCE_saveVariable(%brick,%type,%vars,%client)
{
	%client.eventProcessingObj = %brick;
	if(!isObject(%client))
		return;
	if(isObject(%brick.getGroup().vargroup))
	{
		%category = getField($VCE::Sever::VarType[%type],1);
		%target = eval("return" SPC getField($VCE::Sever::VarType[%type],2) @ "\;");
		if(!isObject(%target))
			return;
		%vargroup = %brick.getGroup().vargroup;
		%vars = strReplace(%vars,",","\t");
		%count = getFieldCount(%vars);
		for(%i=0;%i<%count;%i++)
			%vargroup.saveVariable(%category,trim(getField(%vars,%i),%target));
	}
}
function fxDtsBrick::VCE_loadVariable(%brick,%type,%vars,%client)
{
	%client.eventProcessingObj = %brick;
	if(!isObject(%client))
		return;
	if(isObject(%brick.getGroup().vargroup))
	{
		%category = getField($VCE::Sever::VarType[%type],1);
		%target = eval("return" SPC getField($VCE::Sever::VarType[%type],2) @ "\;");
		if(!isObject(%target))
			return;
		%vargroup = %brick.getGroup().vargroup;
		%vars = strReplace(%vars,",","\t");
		%count = getFieldCount(%vars);
		for(%i=0;%i<%count;%i++)
			%vargroup.loadVariable(%category,trim(getField(%vars,%i),%target));
	}
}
