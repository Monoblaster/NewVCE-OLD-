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
	function gameConnection::ChatMessage(%client,%msg)
	{
		%obj = %client.eventProcessingObj;
		if(isObject(%obj))
			return Parent::ChatMessage(%client,%client.eventProcessingObj.filterVCEString(%msg,%client));
		return Parent::ChatMessage(%client,%msg);
	}
	function gameConnection::CenterPrint(%client,%msg,%time)
	{
		%obj = %client.eventProcessingObj;
		if(isObject(%obj))	
			return Parent::CenterPrint(%client,%client.eventProcessingObj.filterVCEString(%msg,%client), %time);
		return Parent::CenterPrint(%client,%msg,%time);
	}
	function gameConnection::BottomPrint(%client,%msg,%time,%hideBar)
	{
		%obj = %client.eventProcessingObj;
		if(isObject(%obj))
			return Parent::BottomPrint(%client,%client.eventProcessingObj.filterVCEString(%msg,%client), %time, %hideBar);
		return Parent::BottomPrint(%client,%msg,%time, %hideBar);
	}
	function miniGameSO::BottomPrintAll(%mini,%msg,%time,%client)
	{
		if(isObject(%client))
			%msg = strReplace(%msg,"%1",%client.getPlayerName());
		%obj = %client.eventProcessingObj;
		if(isObject(%obj)){
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::BottomPrint(%mini.getObject(%i),%client.eventProcessingObj.filterVCEString(%msg,%mini.getObject(%i)), %time, 0);
		} else{
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::BottomPrint(%mini.getObject(%i),%msg,%time, 0);
		}
	}
	function miniGameSO::CenterPrintAll(%mini,%msg,%time,%client)
	{
		if(isObject(%client))
			%msg = strReplace(%msg,"%1",%client.getPlayerName());
		%obj = %client.eventProcessingObj;
		if(isObject(%obj)){
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::CenterPrint(%mini.getObject(%i),%client.eventProcessingObj.filterVCEString(%msg,%mini.getObject(%i)), %time);
		} else{
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::CenterPrint(%client,%msg,%time);
		}
	}
	function miniGameSO::ChatMsgAll(%mini,%msg,%client)
	{
		%obj = %client.eventProcessingObj;
		%client.eventProcessingObj = 0;
		if(isObject(%obj)){
			for(%i=0;%i<%mini.numMembers;%i++)
				Parent::ChatMessage(%mini.getObject(%i),%client.eventProcessingObj.filterVCEString(%msg,%mini.getObject(%i)));
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
			if(isObject(%brick.getGroup().vargroup))
			{
				if(getWordCount(%par2) != 2)
					return;
				%name = %brick.filterVCEString(%par1,%client);
				%subStart = mClamp(getWord(%par2,0),0,%brick.numEvents);
				%subEnd = mClamp(getWord(%par2,1),0,%brick.numEvents);
				if(%subStart == 0 && %subEnd == 0){
					%subStart = 0;
					%subEnd = %brick.numEvents - 1;
				}
				%brick.vceFunction[%name] = %substart SPC %subend;
			}
		}
		//loop checking
		if((%inputName $= "onVariableTrue" || %inputName $= "onVariableFalse") && (%outputName $= "VCE_ifVariable" || %outputName $= "VCE_ifValue") || (%inputName $= "onVariableUpdate" && (%outputName $= "VCE_ifValue" || %outputName $= "VCE_ifVariable")) || (%inputName $= "onVariableUpdate" && %outputName $= "VCE_modVariable")){
			if (%delay < $Pref::VCE::LoopDelay)
				%delay = $Pref::VCE::LoopDelay;
		}
		Parent::serverCmdAddEvent(%client, %enabled, %inputEventIdx, %delay, %targetIdx, %NTNameIdx, %outputEventIdx, %par1, %par2, %par3, %par4);
	}
	function simObject::processInputEvent(%obj, %EventName, %client){
		%client.eventProcessingObj = %obj;
		Parent::processInputEvent(%obj, %eventName, %client);
	}
	function AIPlayer::startHoleLoop(%bot){
		%bot.hIsEnabled = true;
		Parent::startHoleLoop(%bot);
	}
	function AIPlayer::stopHoleLoop(%bot){
		%bot.hIsEnabled = false;
		Parent::stopHoleLoop(%bot);
	}
};
