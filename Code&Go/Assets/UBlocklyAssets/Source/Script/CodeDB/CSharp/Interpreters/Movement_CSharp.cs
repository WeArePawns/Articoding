/****************************************************************************

Functions for interpreting c# code for blocks.

Copyright 2016 sophieml1989@gmail.com

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

****************************************************************************/


using System.Collections;
using UnityEngine;

namespace UBlockly
{
    public class Times
    {
        public static float instructionWaitTime = 1.5f;   
        public static float logicWaitTime = 0.3f;   
    }

    [CodeInterpreter(BlockType = "movement_move_laser")]
    public class Move_Laser_Cmdtor : EnumeratorCmdtor
    {
        
        protected override IEnumerator Execute(Block block)
        {
            CustomEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "AMOUNT", new DataStruct(0));
            yield return ctor;
            DataStruct arg0 = ctor.Data;

            string dir = block.GetFieldValue("DIRECTION");

            string msg = arg0.ToString() + " " + dir;
            MessageManager.Instance.SendMessage(msg, MSG_TYPE.MOVE_LASER);

            yield return new WaitForSeconds(Times.instructionWaitTime);
        }
    }

    [CodeInterpreter(BlockType = "movement_move")]
    public class Move_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            CustomEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "NAME", new DataStruct(0));
            yield return ctor;
            DataStruct arg0 = ctor.Data;    
            
            ctor = CSharp.Interpreter.ValueReturn(block, "AMOUNT", new DataStruct(0));
            yield return ctor;
            DataStruct arg1 = ctor.Data;

            string dir = block.GetFieldValue("DIRECTION");

            string msg = arg0.ToString() +" "+ arg1.ToString() + " " + dir;
            MessageManager.Instance.SendMessage(msg, MSG_TYPE.MOVE);

            yield return new WaitForSeconds(Times.instructionWaitTime);
        }
    }

    [CodeInterpreter(BlockType = "movement_rotate_laser")]
    public class Rotate_Laser_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            CustomEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "AMOUNT", new DataStruct(0));
            yield return ctor;
            DataStruct arg0 = ctor.Data;

            string rot = block.GetFieldValue("ROTATION");

            string msg = arg0.ToString() + " " + rot;
            MessageManager.Instance.SendMessage(msg, MSG_TYPE.ROTATE_LASER);

            yield return new WaitForSeconds(Times.instructionWaitTime);
        }
    }

    [CodeInterpreter(BlockType = "movement_rotate")]
    public class Rotate_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            CustomEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "NAME", new DataStruct(0));
            yield return ctor;
            DataStruct arg0 = ctor.Data;

            ctor = CSharp.Interpreter.ValueReturn(block, "AMOUNT", new DataStruct(0));
            yield return ctor;
            DataStruct arg1 = ctor.Data;

            string rot = block.GetFieldValue("ROTATION");

            string msg = arg0.ToString() + " " + arg1.ToString() + " " + rot;
            MessageManager.Instance.SendMessage(msg, MSG_TYPE.ROTATE);

            yield return new WaitForSeconds(Times.instructionWaitTime);
        }
    }

    [CodeInterpreter(BlockType = "movement_laser_change_intensity")]
    public class Change_Intensity_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            CustomEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "AMOUNT", new DataStruct(0));
            yield return ctor;
            DataStruct arg0 = ctor.Data;
            Number amount = arg0.NumberValue;           

            string msg = "0 " + amount.ToString();
            MessageManager.Instance.SendMessage(msg, MSG_TYPE.CHANGE_INTENSITY);

            yield return new WaitForSeconds(Times.instructionWaitTime);
        }
    }

    [CodeInterpreter(BlockType = "movement_activate_door")]
    public class Activate_Door_Cmdtor : EnumeratorCmdtor
    {

        protected override IEnumerator Execute(Block block)
        {
            CustomEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "ACTIVE", new DataStruct(0));
            yield return ctor;
            DataStruct arg0 = ctor.Data;

            string index = block.GetFieldValue("INDEX");

            string msg = index + " " + arg0.ToString();
            MessageManager.Instance.SendMessage(msg, MSG_TYPE.ACTIVATE_DOOR);

            yield return new WaitForSeconds(Times.instructionWaitTime);
        }
    }
}
