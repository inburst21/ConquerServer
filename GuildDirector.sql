DELETE FROM cq_action WHERE id>=11000 AND id<12000 LIMIT 1000;
DELETE FROM cq_task WHERE id>=11000 AND id<12000 LIMIT 1000;

INSERT INTO `cq_action` VALUES ('11000', '11001', '0000', '0101', '0', 'If your character is level 90 or above, you can talk to your friends about setting up a Guild. It costs 1,000,000 ');
INSERT INTO `cq_action` VALUES ('11001', '11002', '0000', '0101', '0', 'silvers to set up a guild.');
INSERT INTO `cq_action` VALUES ('11002', '11003', '11004', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11003', '11012', '0000', '0102', '0', 'Disband~my~guild. 11150');
INSERT INTO `cq_action` VALUES ('11012', '11006', '0000', '0102', '0', 'Abdicate~Leadership. 11500');
INSERT INTO `cq_action` VALUES ('11004', '11005', '11052', '1001', '0', 'rankshow < 1');
INSERT INTO `cq_action` VALUES ('11005', '11058', '0000', '0102', '0', 'Create~a~guild. 11050');
INSERT INTO `cq_action` VALUES ('11006', '11007', '0000', '0102', '0', 'Add~alliance. 11200');
INSERT INTO `cq_action` VALUES ('11007', '11008', '0000', '0102', '0', 'Add~enemy. 11250');
INSERT INTO `cq_action` VALUES ('11008', '11009', '0000', '0102', '0', 'Promote~Deputy~Leader. 11300');
INSERT INTO `cq_action` VALUES ('11009', '11010', '0000', '0102', '0', 'Demote~Deputy~Leader. 11350');
INSERT INTO `cq_action` VALUES ('11013', '610007', '0000', '0102', '0', 'More~options. 11020');

INSERT INTO `cq_action` VALUES ('11020', '11021', '0000', '0101', '0', 'What can I do for you?');
INSERT INTO `cq_action` VALUES ('11021', '11022', '0000', '0102', '0', 'Remove~Ally. 11400');
INSERT INTO `cq_action` VALUES ('11022', '11023', '0000', '0102', '0', 'Remove~Enemy. 11450');
INSERT INTO `cq_action` VALUES ('11023', '610007', '0000', '0102', '0', 'More~options. 11000');
/* Guild Creation */
INSERT INTO `cq_action` VALUES ('11050', '11051', '11053', '1001', '0', 'level < 90');
INSERT INTO `cq_action` VALUES ('11051', '11052', '0000', '0101', '0', 'You should be at least level 90 to create a guild.');
INSERT INTO `cq_action` VALUES ('11052', '610007', '0000', '0102', '0', 'I~see. 0');
INSERT INTO `cq_action` VALUES ('11053', '11054', '11056', '1001', '0', 'money < 1000000');
INSERT INTO `cq_action` VALUES ('11054', '11055', '0000', '0101', '0', 'You don`t have enough money.');
INSERT INTO `cq_action` VALUES ('11055', '610007', '0000', '0102', '0', 'I~see. 0');
INSERT INTO `cq_action` VALUES ('11056', '11057', '0000', '0101', '0', 'What would you like to name your guild? A great name is better than great riches.');
INSERT INTO `cq_action` VALUES ('11057', '11058', '0000', '0103', '0', '15 11100 I~name~my~guild...');
INSERT INTO `cq_action` VALUES ('11058', '610007', '0000', '0102', '0', 'Let~me~think~it~over. 0');

INSERT INTO `cq_action` VALUES ('11100', '11101', '0000', '0701', '0', '90 1000000 500000');
INSERT INTO `cq_action` VALUES ('11101', '11102', '0000', '0101', '0', 'Congratulations! You have created your guild successfully. I believe your guild will be expanded rapidly and enjoy a');
INSERT INTO `cq_action` VALUES ('11102', '11103', '0000', '0101', '0', 'good reputation.');
INSERT INTO `cq_action` VALUES ('11103', '610007', '0000', '0102', '0', 'Thanks. 0');
#Guild Disband
INSERT INTO `cq_action` VALUES ('11150', '11151', '0000', '0101', '0', 'Your guild cannot be retrieved once disbanded. You´d better think it over.');
INSERT INTO `cq_action` VALUES ('11151', '11152', '0000', '0102', '0', 'I~decide~to~disband. 11155');
INSERT INTO `cq_action` VALUES ('11152', '610007', '0000', '0102', '0', 'I~changed~my~mind. 0');
INSERT INTO `cq_action` VALUES ('11155', '0000', '0000', '0702', '0', '');
#Guild add ally
INSERT INTO `cq_action` VALUES ('11200', '11203', '11201', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11201', '11202', '0000', '101', '0', 'Only the Guild Leader is enabled to add new allies.');
INSERT INTO `cq_action` VALUES ('11202', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11203', '11204', '0000', '101', '0', 'Adding a new ally to the guild is a big step. Guilds which work together has higher chances to win. To add a new ally, the ');
INSERT INTO `cq_action` VALUES ('11204', '11205', '0000', '101', '0', 'Leader of the target Guild should be in your team. If there be an extra member, it will not be possible. Are you sure ');
INSERT INTO `cq_action` VALUES ('11205', '11206', '0000', '101', '0', 'you want to add this Guild as your ally?');
INSERT INTO `cq_action` VALUES ('11206', '11207', '0000', '101', '0', 'Yes,~sure. 11210');
INSERT INTO `cq_action` VALUES ('11207', '610007', '0000', '101', '0', 'I´ll~think~it~over. 0');

INSERT INTO `cq_action` VALUES ('11210', '11213', '11211', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11211', '11212', '0000', '101', '0', 'Only the Guild Leader is enabled to add new allies.');
INSERT INTO `cq_action` VALUES ('11212', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11213', '11216', '11214', '713', '0', '');
INSERT INTO `cq_action` VALUES ('11214', '11215', '0000', '101', '0', 'It was not possible to set an alliance.');
INSERT INTO `cq_action` VALUES ('11215', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11216', '11217', '0000', '101', '0', 'Your alliance has been set up.');
INSERT INTO `cq_action` VALUES ('11217', '610007', '0000', '102', '0', 'Great!~Thank~you. 0');
#Guild add enemy
INSERT INTO `cq_action` VALUES ('11250', '11253', '11251', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11251', '11252', '0000', '101', '0', 'Only the Guild Leader is enabled to add new enemies.');
INSERT INTO `cq_action` VALUES ('11252', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11253', '11254', '0000', '101', '0', 'After adding a guild as enemy, they will receive a notification telling them about it. Please write below the ');
INSERT INTO `cq_action` VALUES ('11254', '11255', '0000', '103', '0', '15 11260 Enemy~syndicate~name:');
INSERT INTO `cq_action` VALUES ('11255', '610007', '0000', '102', '0', 'Just~passing~by. 0');

INSERT INTO `cq_action` VALUES ('11260', '11263', '11261', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11261', '11262', '0000', '101', '0', 'Only the Guild Leader is enabled to add new enemies.');
INSERT INTO `cq_action` VALUES ('11262', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11263', '11266', '11264', '711', '0', '');
INSERT INTO `cq_action` VALUES ('11264', '11265', '0000', '101', '0', 'It was not possible to set the guild as enemy.');
INSERT INTO `cq_action` VALUES ('11265', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11266', '11267', '0000', '101', '0', 'The target has been enemied successfully.');
INSERT INTO `cq_action` VALUES ('11267', '610007', '0000', '102', '0', 'Great!~Thank~you. 0');
#Guild promote member
INSERT INTO `cq_action` VALUES ('11300', '11303', '11301', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11301', '11302', '0000', '101', '0', 'Only the Guild Leader is enabled to promote new Deputy Leaders.');
INSERT INTO `cq_action` VALUES ('11302', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11303', '11304', '0000', '101', '0', 'Deputy leaders can accept new members to the guild. They have great responsability with the guild and should ');
INSERT INTO `cq_action` VALUES ('11304', '11305', '0000', '101', '0', 'be choosen wisely. Please name the member that will be promoted. It should be online.');
INSERT INTO `cq_action` VALUES ('11305', '11306', '0000', '103', '0', '16 11310 Name~your~deputy:');
INSERT INTO `cq_action` VALUES ('11306', '610007', '0000', '102', '0', 'Just~passing~by. 0');

INSERT INTO `cq_action` VALUES ('11310', '11313', '11311', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11311', '11312', '0000', '101', '0', 'Only the Guild Leader is enabled to promote new Deputy Leaders.');
INSERT INTO `cq_action` VALUES ('11312', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11313', '11316', '11314', '705', '0', '');
INSERT INTO `cq_action` VALUES ('11314', '11315', '0000', '101', '0', 'It was not possible to promote the target.');
INSERT INTO `cq_action` VALUES ('11315', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11316', '11317', '0000', '101', '0', 'The target has been promoted successfully.');
INSERT INTO `cq_action` VALUES ('11317', '610007', '0000', '102', '0', 'Great!~Thank~you. 0');

#Guild demote member
INSERT INTO `cq_action` VALUES ('11350', '11353', '11351', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11351', '11352', '0000', '101', '0', 'Only the Guild Leader is enabled to demote Deputy Leaders.');
INSERT INTO `cq_action` VALUES ('11352', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11353', '11354', '0000', '101', '0', 'Sometimes we make mistakes. Who´d you want to demote?');
INSERT INTO `cq_action` VALUES ('11354', '11355', '0000', '103', '0', '16 11360 Name~the~target:');
INSERT INTO `cq_action` VALUES ('11355', '610007', '0000', '102', '0', 'Just~passing~by. 0');

INSERT INTO `cq_action` VALUES ('11360', '11363', '11361', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11361', '11362', '0000', '101', '0', 'Only the Guild Leader is enabled to demote Deputy Leaders.');
INSERT INTO `cq_action` VALUES ('11362', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11363', '11366', '11364', '706', '0', '');
INSERT INTO `cq_action` VALUES ('11364', '11365', '0000', '101', '0', 'It was not possible to demote the target.');
INSERT INTO `cq_action` VALUES ('11365', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11366', '11367', '0000', '101', '0', 'The target has been demoted successfully.');
INSERT INTO `cq_action` VALUES ('11367', '610007', '0000', '102', '0', 'Great!~Thank~you. 0');

#Guild remove ally
INSERT INTO `cq_action` VALUES ('11400', '11403', '11401', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11401', '11402', '0000', '101', '0', 'Only the Guild Leader is enabled to remove allies.');
INSERT INTO `cq_action` VALUES ('11402', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11403', '11404', '0000', '101', '0', 'Adding a new ally to the guild is a big step. Guilds which work together has higher chances to win. To add a new ally, the ');
INSERT INTO `cq_action` VALUES ('11404', '11405', '0000', '101', '0', 'Leader of the target Guild should be in your team. If there be an extra member, it will not be possible. Are you sure ');
INSERT INTO `cq_action` VALUES ('11405', '11406', '0000', '101', '0', 'you want to add this Guild as your ally?');
INSERT INTO `cq_action` VALUES ('11406', '11407', '0000', '101', '0', 'Yes,~sure. 11410');
INSERT INTO `cq_action` VALUES ('11407', '610007', '0000', '101', '0', 'I´ll~think~it~over. 0');

INSERT INTO `cq_action` VALUES ('11410', '11413', '11411', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11411', '11412', '0000', '101', '0', 'Only the Guild Leader is enabled to remove allies.');
INSERT INTO `cq_action` VALUES ('11412', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11413', '11416', '11414', '714', '0', '');
INSERT INTO `cq_action` VALUES ('11414', '11415', '0000', '101', '0', 'It was not possible to disband the alliance.');
INSERT INTO `cq_action` VALUES ('11415', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11416', '11417', '0000', '101', '0', 'Your alliance has dismissed.');
INSERT INTO `cq_action` VALUES ('11417', '610007', '0000', '102', '0', 'Great!~Thank~you. 0');
#Guild remove enemy
INSERT INTO `cq_action` VALUES ('11450', '11453', '11451', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11451', '11452', '0000', '101', '0', 'Only the Guild Leader is enabled to remove enemies.');
INSERT INTO `cq_action` VALUES ('11452', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11453', '11454', '0000', '101', '0', 'After adding a guild as enemy, they will receive a notification telling them about it. Please write below the ');
INSERT INTO `cq_action` VALUES ('11454', '11455', '0000', '103', '0', '15 11460 Enemy~syndicate~name:');
INSERT INTO `cq_action` VALUES ('11455', '610007', '0000', '102', '0', 'Just~passing~by. 0');

INSERT INTO `cq_action` VALUES ('11460', '11463', '11461', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11461', '11462', '0000', '101', '0', 'Only the Guild Leader is enabled to remove enemies.');
INSERT INTO `cq_action` VALUES ('11462', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11463', '11466', '11464', '712', '0', '');
INSERT INTO `cq_action` VALUES ('11464', '11465', '0000', '101', '0', 'It was not possible to remove the guild from enemy.');
INSERT INTO `cq_action` VALUES ('11465', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11466', '11467', '0000', '101', '0', 'The target has been removed from enemies successfully.');
INSERT INTO `cq_action` VALUES ('11467', '610007', '0000', '102', '0', 'Great!~Thank~you. 0');
#Guild abdicate
INSERT INTO `cq_action` VALUES ('11500', '11503', '11501', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11501', '11502', '0000', '101', '0', 'Only the Guild Leader can abdicate the guild.');
INSERT INTO `cq_action` VALUES ('11502', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11503', '11504', '0000', '101', '0', 'After abdicating the guild, you cannot claim it back. Are you sure you want to pass the leadership to:');
INSERT INTO `cq_action` VALUES ('11504', '11505', '0000', '103', '0', '15 11510 Enter~the~member~name:');
INSERT INTO `cq_action` VALUES ('11505', '610007', '0000', '102', '0', 'Just~passing~by. 0');

INSERT INTO `cq_action` VALUES ('11510', '11513', '11511', '1001', '0', 'rankshow == 100');
INSERT INTO `cq_action` VALUES ('11511', '11512', '0000', '101', '0', 'Only the Guild Leader can abdicate the guild.');
INSERT INTO `cq_action` VALUES ('11512', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11513', '11516', '11514', '709', '0', '');
INSERT INTO `cq_action` VALUES ('11514', '11515', '0000', '101', '0', 'It was not possible to abdicate the guild.');
INSERT INTO `cq_action` VALUES ('11515', '610007', '0000', '102', '0', 'Oh,~I~see. 0');
INSERT INTO `cq_action` VALUES ('11516', '11517', '0000', '101', '0', 'The leadership has been moved successfully.');
INSERT INTO `cq_action` VALUES ('11517', '610007', '0000', '102', '0', 'Great!~Thank~you. 0');

INSERT INTO `cq_task` VALUES ('11000', '11000', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11050', '11050', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11100', '11100', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11150', '11150', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11200', '11200', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11250', '11250', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11300', '11300', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11350', '11350', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11155', '11155', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11210', '11210', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11260', '11260', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11310', '11310', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11360', '11360', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11400', '11400', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11410', '11410', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11450', '11450', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11460', '11460', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11020', '11020', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11500', '11500', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('11510', '11510', '0000', '', '', '0', '0', '0', '-65535', '65535', '0000', '0000', '0', '-1', '0');